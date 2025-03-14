// Copyright (C) 2015-2025 The Neo Project.
//
// KeyPair.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.Cryptography;
using Neo.SmartContract;
using Neo.Wallets.NEP6;
using Org.BouncyCastle.Crypto.Generators;
using System;
using System.Security.Cryptography;
using System.Text;
using static Neo.Wallets.Helper;
using ECCurve = Neo.Cryptography.ECC.ECCurve;
using ECPoint = Neo.Cryptography.ECC.ECPoint;

namespace Neo.Wallets
{
    /// <summary>
    /// Represents a private/public key pair in wallets.
    /// </summary>
    public class KeyPair : IEquatable<KeyPair>
    {
        /// <summary>
        /// The private key.
        /// </summary>
        public readonly byte[] PrivateKey;

        /// <summary>
        /// The public key.
        /// </summary>
        public readonly ECPoint PublicKey;

        /// <summary>
        /// The hash of the public key.
        /// </summary>
        public UInt160 PublicKeyHash => PublicKey.EncodePoint(true).ToScriptHash();

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyPair"/> class.
        /// </summary>
        /// <param name="privateKey">The private key in the <see cref="KeyPair"/>.</param>
        public KeyPair(byte[] privateKey)
        {
            if (privateKey.Length != 32 && privateKey.Length != 96 && privateKey.Length != 104)
                throw new ArgumentException(null, nameof(privateKey));
            PrivateKey = privateKey[^32..];
            if (privateKey.Length == 32)
            {
                PublicKey = ECCurve.Secp256r1.G * privateKey;
            }
            else
            {
                PublicKey = ECPoint.FromBytes(privateKey, ECCurve.Secp256r1);
            }
        }

        public bool Equals(KeyPair other)
        {
            if (ReferenceEquals(this, other)) return true;
            if (other is null) return false;
            return PublicKey.Equals(other.PublicKey);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as KeyPair);
        }

        /// <summary>
        /// Exports the private key in WIF format.
        /// </summary>
        /// <returns>The private key in WIF format.</returns>
        public string Export()
        {
            Span<byte> data = stackalloc byte[34];
            data[0] = 0x80;
            PrivateKey.CopyTo(data[1..]);
            data[33] = 0x01;
            string wif = Base58.Base58CheckEncode(data);
            data.Clear();
            return wif;
        }

        /// <summary>
        /// Exports the private key in NEP-2 format.
        /// </summary>
        /// <param name="passphrase">The passphrase of the private key.</param>
        /// <param name="version">The address version.</param>
        /// <param name="N">The N field of the <see cref="ScryptParameters"/> to be used.</param>
        /// <param name="r">The R field of the <see cref="ScryptParameters"/> to be used.</param>
        /// <param name="p">The P field of the <see cref="ScryptParameters"/> to be used.</param>
        /// <returns>The private key in NEP-2 format.</returns>
        public string Export(string passphrase, byte version, int N = 16384, int r = 8, int p = 8)
        {
            byte[] passphrasedata = Encoding.UTF8.GetBytes(passphrase);
            try
            {
                return Export(passphrasedata, version, N, r, p);
            }
            finally
            {
                passphrasedata.AsSpan().Clear();
            }
        }

        /// <summary>
        /// Exports the private key in NEP-2 format.
        /// </summary>
        /// <param name="passphrase">The passphrase of the private key.</param>
        /// <param name="version">The address version.</param>
        /// <param name="N">The N field of the <see cref="ScryptParameters"/> to be used.</param>
        /// <param name="r">The R field of the <see cref="ScryptParameters"/> to be used.</param>
        /// <param name="p">The P field of the <see cref="ScryptParameters"/> to be used.</param>
        /// <returns>The private key in NEP-2 format.</returns>
        public string Export(byte[] passphrase, byte version, int N = 16384, int r = 8, int p = 8)
        {
            UInt160 script_hash = Contract.CreateSignatureRedeemScript(PublicKey).ToScriptHash();
            string address = script_hash.ToAddress(version);
            byte[] addresshash = Encoding.ASCII.GetBytes(address).Sha256().Sha256()[..4];
            byte[] derivedkey = SCrypt.Generate(passphrase, addresshash, N, r, p, 64);
            byte[] derivedhalf1 = derivedkey[..32];
            byte[] derivedhalf2 = derivedkey[32..];
            byte[] encryptedkey = Encrypt(XOR(PrivateKey, derivedhalf1), derivedhalf2);
            Span<byte> buffer = stackalloc byte[39];
            buffer[0] = 0x01;
            buffer[1] = 0x42;
            buffer[2] = 0xe0;
            addresshash.CopyTo(buffer[3..]);
            encryptedkey.CopyTo(buffer[7..]);
            return Base58.Base58CheckEncode(buffer);
        }

        private static byte[] Encrypt(byte[] data, byte[] key)
        {
            using Aes aes = Aes.Create();
            aes.Key = key;
            aes.Mode = CipherMode.ECB;
            aes.Padding = PaddingMode.None;
            using ICryptoTransform encryptor = aes.CreateEncryptor();
            return encryptor.TransformFinalBlock(data, 0, data.Length);
        }

        public override int GetHashCode()
        {
            return PublicKey.GetHashCode();
        }

        public override string ToString()
        {
            return PublicKey.ToString();
        }
    }
}
