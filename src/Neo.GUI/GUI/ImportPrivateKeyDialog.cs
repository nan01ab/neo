// Copyright (C) 2015-2025 The Neo Project.
//
// ImportPrivateKeyDialog.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Neo.GUI
{
    internal partial class ImportPrivateKeyDialog : Form
    {
        public ImportPrivateKeyDialog()
        {
            InitializeComponent();
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string[] WifStrings
        {
            get
            {
                return textBox1.Lines;
            }
            set
            {
                textBox1.Lines = value;
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            button1.Enabled = textBox1.TextLength > 0;
        }
    }
}
