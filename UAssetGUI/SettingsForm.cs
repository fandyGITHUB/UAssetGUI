﻿using System;
using System.Drawing;
using System.Windows.Forms;

namespace UAssetGUI
{
    public partial class SettingsForm : Form
    {
        public SettingsForm()
        {
            InitializeComponent();
        }

        private Form1 BaseForm;
        private bool _readyToUpdateTheme = false;
        private void SettingsForm_Load(object sender, EventArgs e)
        {
            if (this.Owner is Form1) BaseForm = (Form1)this.Owner;
            themeComboBox.DataSource = Enum.GetValues(typeof(UAGTheme));
            themeComboBox.SelectedIndex = (int)UAGPalette.GetCurrentTheme();
            valuesOnScroll.Checked = UAGConfig.Data.ChangeValuesOnScroll;
            doubleClickToEdit.Checked = UAGConfig.Data.DoubleClickToEdit;
            enableDiscordRpc.Checked = UAGConfig.Data.EnableDiscordRPC;
            enableDynamicTree.Checked = UAGConfig.Data.EnableDynamicTree;
            favoriteThingBox.Text = UAGConfig.Data.FavoriteThing;
            numericUpDown1.Value = UAGConfig.Data.DataZoom;

            UAGPalette.RefreshTheme(this);
            this.AdjustFormPosition();
            _readyToUpdateTheme = true;
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private static string AboutText;

        private void aboutButton_Click(object sender, EventArgs e)
        {
            AboutText = (this.Owner as Form1).DisplayVersion + "\n" +
            "By atenfyr\n" +
            "\nThanks to Kaiheilos for early assistance with the package file summary format\n" +
            "\n(Here's where a soppy monologue goes)\n";

            var formPopup = new Form
            {
                Text = "About",
                Size = new Size(350, 300)
            };

            formPopup.Controls.Add(new Label()
            {
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill,
                Text = AboutText,
                Font = new Font(this.Font.FontFamily, 10)
            });

            UAGPalette.RefreshTheme(formPopup);

            formPopup.StartPosition = FormStartPosition.CenterParent;
            formPopup.ShowDialog(this);
        }

        private void themeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_readyToUpdateTheme) return;
            Enum.TryParse(themeComboBox.SelectedValue.ToString(), out UAGTheme nextTheme);
            UAGPalette.SetCurrentTheme(nextTheme);
            UAGPalette.RefreshTheme(BaseForm);
            UAGPalette.RefreshTheme(this);
        }

        private void valuesOnScroll_CheckedChanged(object sender, EventArgs e)
        {
            UAGConfig.Data.ChangeValuesOnScroll = valuesOnScroll.Checked;
        }

        private void enableDynamicTree_CheckedChanged(object sender, EventArgs e)
        {
            UAGConfig.Data.EnableDynamicTree = enableDynamicTree.Checked;
        }

        private void doubleClickToEdit_CheckedChanged(object sender, EventArgs e)
        {
            UAGConfig.Data.DoubleClickToEdit = doubleClickToEdit.Checked;
            //BaseForm.dataGridView1.EditMode = UAGConfig.Data.DoubleClickToEdit ? DataGridViewEditMode.EditProgrammatically : DataGridViewEditMode.EditOnEnter;
        }

        private void enableDiscordRpc_CheckedChanged(object sender, EventArgs e)
        {
            UAGConfig.Data.EnableDiscordRPC = enableDiscordRpc.Checked;
            if (UAGConfig.Data.EnableDiscordRPC)
            {
                BaseForm.UpdateRPC();
            }
            else
            {
                BaseForm.DiscordRPC.ClearPresence();
            }
        }

        private bool isCurrentlyComicSans = false;
        private void favoriteThingBox_TextChanged(object sender, EventArgs e)
        {
            UAGConfig.Data.FavoriteThing = favoriteThingBox.Text;
            if (UAGConfig.Data.FavoriteThing.ToLowerInvariant().StartsWith("comic sans"))
            {
                isCurrentlyComicSans = true;
                UAGPalette.RefreshTheme(BaseForm);
                UAGPalette.RefreshTheme(this);
            }
            else if (isCurrentlyComicSans)
            {
                isCurrentlyComicSans = false;
                UAGPalette.RefreshTheme(BaseForm);
                UAGPalette.RefreshTheme(this);
            }
        }

        private void SettingsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            UAGConfig.Save();
            UAGPalette.RefreshTheme(BaseForm);
            UAGPalette.RefreshTheme(this);
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            UAGConfig.Data.DataZoom = (int)numericUpDown1.Value;
            UAGPalette.RefreshTheme(BaseForm);
            UAGPalette.RefreshTheme(this);

            // Refresh dgv row heights
            UAGUtils.InvokeUI(() =>
            {
                if (BaseForm.tableEditor != null)
                {
                    BaseForm.tableEditor.Save(true);
                }
            });
        }
    }
}
