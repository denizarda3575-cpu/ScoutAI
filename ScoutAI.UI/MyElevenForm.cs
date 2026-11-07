using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using ScoutAI.DAL;
using ScoutAI.Entities;

namespace ScoutAI.UI
{
    public partial class MyElevenForm : Form
    {
        private readonly PlayerRepository _playerRepository = new PlayerRepository();

        private Panel pnlPitch;
        private ComboBox cmbFormations;
        private Label lblTitle;
        private FlowLayoutPanel pnlPlayerPool;
        private Label lblPoolTitle;

        // Taktiksel Roller ve Medikal Paneli
        private Panel pnlTacticalRoles;
        private ComboBox cmbCaptain;
        private ComboBox cmbRightCorner;
        private ComboBox cmbLeftCorner;
        private ComboBox cmbPenalty;
        private Button btnMedicalProcess;

        // Yedek Kulübesi
        private Panel pnlSubsContainer;
        private Button bntToggleSubs;
        private FlowLayoutPanel pnlSubsList;
        private bool isSubsExpanded = false;

        private List<Button> pitchSlots = new List<Button>();
        private Button selectedSlot = null;
        private List<PlayerDto> userPlayers = new List<PlayerDto>();
        private bool isInitializing = true;

        public MyElevenForm()
        {
            InitializeComponentCustom();
            SetupPitchSlots();
            ApplyFormation(cmbFormations.SelectedIndex);
            LoadUserPlayersAndRestoreLineup();
        }

        private void InitializeComponentCustom()
        {
            this.Text = "Scout AI - Benim 11'im (Taktik ve Kadro Planlama)";
            this.Size = new Size(1180, 880);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(20, 25, 35);

            lblTitle = new Label()
            {
                Text = "TAKTİKSEL İLK 11 PLANLAMA",
                ForeColor = Color.LightGreen,
                Font = new Font("Arial", 16, FontStyle.Bold),
                Location = new Point(30, 15),
                AutoSize = true
            };

            Label lblFormation = new Label()
            {
                Text = "Diziliş Seçin:",
                ForeColor = Color.White,
                Font = new Font("Arial", 10, FontStyle.Bold),
                Location = new Point(30, 55),
                AutoSize = true
            };

            cmbFormations = new ComboBox()
            {
                Location = new Point(130, 52),
                Width = 220,
                Font = new Font("Arial", 10, FontStyle.Regular),
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            cmbFormations.Items.Add("1-4-4-2 (Klasik Dengeli)");
            cmbFormations.Items.Add("1-4-3-3 (Ofansif Kanat)");
            cmbFormations.Items.Add("1-4-2-3-1 (Modern Oyun Kurucu)");
            cmbFormations.Items.Add("1-3-5-2 (Orta Saha Hakimiyeti)");
            cmbFormations.Items.Add("1-5-3-2 (Katı Savunma)");
            cmbFormations.Items.Add("1-4-1-4-1 (Dirençli Merkez)");
            cmbFormations.Items.Add("1-3-4-3 (Agresif Üçlü)");
            cmbFormations.Items.Add("1-4-3-2-1 (Noel Ağacı)");
            cmbFormations.Items.Add("1-4-2-4 (Çift Forvet Baskı)");
            cmbFormations.Items.Add("1-5-4-1 (Defansif Blok)");
            cmbFormations.SelectedIndex = 0;
            cmbFormations.SelectedIndexChanged += CmbFormations_SelectedIndexChanged;

            pnlPitch = new Panel()
            {
                Location = new Point(30, 90),
                Width = 680,
                Height = 540,
                BackColor = Color.FromArgb(34, 139, 34)
            };
            pnlPitch.Paint += PnlPitch_Paint;

            lblPoolTitle = new Label()
            {
                Text = "KAYITLI OYUNCULARIM (Havuz)",
                ForeColor = Color.White,
                Font = new Font("Arial", 11, FontStyle.Bold),
                Location = new Point(735, 15),
                AutoSize = true
            };

            pnlPlayerPool = new FlowLayoutPanel()
            {
                Location = new Point(735, 45),
                Width = 400,
                Height = 390,
                AutoScroll = true,
                BackColor = Color.FromArgb(28, 35, 48)
            };

            pnlTacticalRoles = new Panel()
            {
                Location = new Point(735, 445),
                Width = 400,
                Height = 375,
                BackColor = Color.FromArgb(28, 35, 48)
            };

            Label lblTacHeader = new Label()
            {
                Text = "⚡ TAKTİKSEL ROLLER & MEDİKAL",
                ForeColor = Color.Gold,
                Font = new Font("Arial", 9.5f, FontStyle.Bold),
                Location = new Point(10, 10),
                AutoSize = true
            };

            Label lblCap = new Label() { Text = "Takım Kaptanı (C):", ForeColor = Color.LightGray, Font = new Font("Arial", 8.5f), Location = new Point(10, 42), AutoSize = true };
            cmbCaptain = new ComboBox() { Location = new Point(150, 40), Width = 230, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbCaptain.SelectedIndexChanged += CmbCaptain_SelectedIndexChanged;

            Label lblRC = new Label() { Text = "Sağ Korner (R):", ForeColor = Color.LightGray, Font = new Font("Arial", 8.5f), Location = new Point(10, 77), AutoSize = true };
            cmbRightCorner = new ComboBox() { Location = new Point(150, 75), Width = 230, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbRightCorner.SelectedIndexChanged += CmbRightCorner_SelectedIndexChanged;

            Label lblLC = new Label() { Text = "Sol Korner (L):", ForeColor = Color.LightGray, Font = new Font("Arial", 8.5f), Location = new Point(10, 112), AutoSize = true };
            cmbLeftCorner = new ComboBox() { Location = new Point(150, 110), Width = 230, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbLeftCorner.SelectedIndexChanged += CmbLeftCorner_SelectedIndexChanged;

            Label lblPen = new Label() { Text = "Penaltı Atıcısı (⚽):", ForeColor = Color.LightGray, Font = new Font("Arial", 8.5f), Location = new Point(10, 147), AutoSize = true };
            cmbPenalty = new ComboBox() { Location = new Point(150, 145), Width = 230, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbPenalty.SelectedIndexChanged += CmbPenalty_SelectedIndexChanged;

            btnMedicalProcess = new Button()
            {
                Text = "🏥 Sakat Oyuncuları Yönet / İyileştir",
                Left = 10,
                Top = 190,
                Width = 370,
                Height = 38,
                BackColor = Color.FromArgb(231, 76, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Arial", 9, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnMedicalProcess.FlatAppearance.BorderSize = 0;
            btnMedicalProcess.Click += BtnMedicalProcess_Click;

            pnlTacticalRoles.Controls.Add(lblTacHeader);
            pnlTacticalRoles.Controls.Add(lblCap);
            pnlTacticalRoles.Controls.Add(cmbCaptain);
            pnlTacticalRoles.Controls.Add(lblRC);
            pnlTacticalRoles.Controls.Add(cmbRightCorner);
            pnlTacticalRoles.Controls.Add(lblLC);
            pnlTacticalRoles.Controls.Add(cmbLeftCorner);
            pnlTacticalRoles.Controls.Add(lblPen);
            pnlTacticalRoles.Controls.Add(cmbPenalty);
            pnlTacticalRoles.Controls.Add(btnMedicalProcess);

            pnlSubsContainer = new Panel()
            {
                Location = new Point(30, 640),
                Width = 680,
                Height = 120,
                BackColor = Color.FromArgb(28, 35, 48)
            };

            bntToggleSubs = new Button()
            {
                Text = "⚽ Yedek Kulübesini Göster / Gizle",
                Left = 10,
                Top = 8,
                Width = 220,
                Height = 28,
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Arial", 8.5f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            bntToggleSubs.FlatAppearance.BorderSize = 0;
            bntToggleSubs.Click += BntToggleSubs_Click;

            pnlSubsList = new FlowLayoutPanel()
            {
                Location = new Point(10, 40),
                Width = 660,
                Height = 70,
                AutoScroll = true,
                Visible = false,
                BackColor = Color.FromArgb(35, 45, 60)
            };

            pnlSubsContainer.Controls.Add(bntToggleSubs);
            pnlSubsContainer.Controls.Add(pnlSubsList);

            this.Controls.Add(lblTitle);
            this.Controls.Add(lblFormation);
            this.Controls.Add(cmbFormations);
            this.Controls.Add(pnlPitch);
            this.Controls.Add(lblPoolTitle);
            this.Controls.Add(pnlPlayerPool);
            this.Controls.Add(pnlTacticalRoles);
            this.Controls.Add(pnlSubsContainer);
        }

        private void BntToggleSubs_Click(object sender, EventArgs e)
        {
            isSubsExpanded = !isSubsExpanded;
            if (isSubsExpanded)
            {
                pnlSubsContainer.Height = 180;
                pnlSubsList.Height = 130;
                pnlSubsList.Visible = true;
                bntToggleSubs.Text = "▲ Yedek Kulübesini Gizle";
            }
            else
            {
                pnlSubsContainer.Height = 120;
                pnlSubsList.Visible = false;
                bntToggleSubs.Text = "⚽ Yedek Kulübesini Göster / Gizle";
            }
        }

        private void SetupPitchSlots()
        {
            for (int i = 0; i < 11; i++)
            {
                Button btnSlot = new Button()
                {
                    Size = new Size(100, 40),
                    BackColor = Color.FromArgb(40, 50, 65),
                    ForeColor = Color.Gold,
                    Font = new Font("Arial", 7.5f, FontStyle.Bold),
                    FlatStyle = FlatStyle.Flat,
                    Text = "BOŞ",
                    Tag = null
                };
                btnSlot.FlatAppearance.BorderColor = Color.LightGray;
                btnSlot.FlatAppearance.BorderSize = 1;
                btnSlot.Click += PitchSlot_Click;

                pitchSlots.Add(btnSlot);
                pnlPitch.Controls.Add(btnSlot);
            }
        }

        private void LoadUserPlayersAndRestoreLineup()
        {
            isInitializing = true;
            userPlayers = _playerRepository.GetAllPlayersWithTeam();
            pnlPlayerPool.Controls.Clear();
            pnlSubsList.Controls.Clear();

            cmbCaptain.Items.Clear();
            cmbRightCorner.Items.Clear();
            cmbLeftCorner.Items.Clear();
            cmbPenalty.Items.Clear();

            foreach (var slot in pitchSlots)
            {
                slot.Text = "BOŞ";
                slot.Tag = null;
                slot.BackColor = Color.FromArgb(40, 50, 65);
                slot.ForeColor = Color.Gold;
            }

            int captainIndex = -1;
            int rcIndex = -1;
            int lcIndex = -1;
            int penIndex = -1;
            int indexCounter = 0;

            if (userPlayers != null && userPlayers.Count > 0)
            {
                foreach (var p in userPlayers)
                {
                    cmbCaptain.Items.Add(new KeyValuePair<int, string>(p.PlayerId, p.FullName));
                    cmbRightCorner.Items.Add(new KeyValuePair<int, string>(p.PlayerId, p.FullName));
                    cmbLeftCorner.Items.Add(new KeyValuePair<int, string>(p.PlayerId, p.FullName));
                    cmbPenalty.Items.Add(new KeyValuePair<int, string>(p.PlayerId, p.FullName));

                    if (p.IsCaptain) captainIndex = indexCounter;
                    if (p.SetPieceRole == "RightCorner") rcIndex = indexCounter;
                    if (p.SetPieceRole == "LeftCorner") lcIndex = indexCounter;
                    if (p.SetPieceRole == "Penalty") penIndex = indexCounter;

                    indexCounter++;

                    if (p.IsInjured) continue;

                    if (p.SquadStatus == "FirstEleven" && p.SlotIndex >= 0 && p.SlotIndex < 11)
                    {
                        Button targetSlot = pitchSlots[p.SlotIndex];

                        string badges = "";
                        if (p.IsCaptain) badges += " [C]";
                        if (p.SetPieceRole == "RightCorner") badges += " [R]";
                        if (p.SetPieceRole == "LeftCorner") badges += " [L]";
                        if (p.SetPieceRole == "Penalty") badges += " [⚽]";

                        targetSlot.Text = $"{p.FullName}{badges}\n({p.Position})";
                        targetSlot.Tag = p;
                        targetSlot.BackColor = Color.FromArgb(46, 204, 113);
                        targetSlot.ForeColor = Color.Black;
                    }

                    if (p.SquadStatus == "Pool")
                    {
                        AddPlayerToPoolUI(p);
                    }

                    if (p.SquadStatus == "Substitute")
                    {
                        AddPlayerToSubsUI(p);
                    }
                }

                cmbCaptain.DisplayMember = "Value"; cmbCaptain.ValueMember = "Key";
                cmbRightCorner.DisplayMember = "Value"; cmbRightCorner.ValueMember = "Key";
                cmbLeftCorner.DisplayMember = "Value"; cmbLeftCorner.ValueMember = "Key";
                cmbPenalty.DisplayMember = "Value"; cmbPenalty.ValueMember = "Key";

                if (captainIndex >= 0) cmbCaptain.SelectedIndex = captainIndex;
                if (rcIndex >= 0) cmbRightCorner.SelectedIndex = rcIndex;
                if (lcIndex >= 0) cmbLeftCorner.SelectedIndex = lcIndex;
                if (penIndex >= 0) cmbPenalty.SelectedIndex = penIndex;
            }
            else
            {
                Label lblEmpty = new Label()
                {
                    Text = "Kayıtlı oyuncunuz bulunmamaktadır.",
                    ForeColor = Color.Gray,
                    Font = new Font("Arial", 9.5f, FontStyle.Italic),
                    Location = new Point(10, 20),
                    AutoSize = true
                };
                pnlPlayerPool.Controls.Add(lblEmpty);
            }

            isInitializing = false;
        }

        private void AddPlayerToPoolUI(PlayerDto p)
        {
            Panel card = new Panel()
            {
                Width = 370,
                Height = 45,
                BackColor = Color.FromArgb(45, 55, 72),
                Margin = new Padding(5),
                Cursor = Cursors.Hand
            };

            Label lblName = new Label()
            {
                Text = p.FullName,
                ForeColor = Color.White,
                Font = new Font("Arial", 9f, FontStyle.Bold),
                Location = new Point(10, 6),
                AutoSize = true
            };

            Label lblPos = new Label()
            {
                Text = $"Mevki: {p.Position}",
                ForeColor = Color.FromArgb(46, 204, 113),
                Font = new Font("Arial", 8f, FontStyle.Regular),
                Location = new Point(10, 24),
                AutoSize = true
            };

            card.Controls.Add(lblName);
            card.Controls.Add(lblPos);

            card.Click += (s, e) => {
                if (selectedSlot != null)
                {
                    AssignPlayerToSelectedSlot(p);
                }
                else
                {
                    var choice = MessageBox.Show($"{p.FullName} oyuncusu için işlem seçin:\n\n[Evet] -> Yedek Kulübesine At\n[Hayır] -> Sakatlık Departmanına Gönder", "Oyuncu Menüsü", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (choice == DialogResult.Yes)
                    {
                        _playerRepository.UpdatePlayerSquadStatus(p.PlayerId, "Substitute", -1);
                        LoadUserPlayersAndRestoreLineup();
                    }
                    else if (choice == DialogResult.No)
                    {
                        _playerRepository.UpdatePlayerInjuryStatus(p.PlayerId, true);
                        _playerRepository.UpdatePlayerSquadStatus(p.PlayerId, "Pool", -1);
                        LoadUserPlayersAndRestoreLineup();
                    }
                }
            };

            pnlPlayerPool.Controls.Add(card);
        }

        private void AddPlayerToSubsUI(PlayerDto p)
        {
            Button btnSub = new Button()
            {
                Text = $"{p.FullName}\n({p.Position})",
                Width = 120,
                Height = 55,
                BackColor = Color.FromArgb(230, 126, 34),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Arial", 7.5f, FontStyle.Bold),
                Cursor = Cursors.Hand,
                Tag = p
            };
            btnSub.FlatAppearance.BorderSize = 0;

            btnSub.Click += (s, e) => {
                if (selectedSlot != null)
                {
                    AssignPlayerToSelectedSlot(p);
                }
                else
                {
                    var choice = MessageBox.Show($"{p.FullName} için işlem seçin:\n\n[Evet] -> Ana Havuza Gönder\n[Hayır] -> Sakatlığa Al", "Yedek Menüsü", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (choice == DialogResult.Yes)
                    {
                        _playerRepository.UpdatePlayerSquadStatus(p.PlayerId, "Pool", -1);
                        LoadUserPlayersAndRestoreLineup();
                    }
                    else if (choice == DialogResult.No)
                    {
                        _playerRepository.UpdatePlayerInjuryStatus(p.PlayerId, true);
                        _playerRepository.UpdatePlayerSquadStatus(p.PlayerId, "Pool", -1);
                        LoadUserPlayersAndRestoreLineup();
                    }
                }
            };

            pnlSubsList.Controls.Add(btnSub);
        }

        private void PitchSlot_Click(object sender, EventArgs e)
        {
            Button clickedSlot = sender as Button;

            if (clickedSlot.Tag != null && selectedSlot == null)
            {
                PlayerDto currentP = clickedSlot.Tag as PlayerDto;

                var action = MessageBox.Show($"{currentP.FullName} ({currentP.Position})\n\n[Evet] -> Havuza At\n[Hayır] -> Yedeğe At\n[İptal] -> Sakatlığa Gönder",
                    "Kadro Yönetimi", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                if (action == DialogResult.Yes)
                {
                    _playerRepository.UpdatePlayerSquadStatus(currentP.PlayerId, "Pool", -1);
                    LoadUserPlayersAndRestoreLineup();
                    return;
                }
                else if (action == DialogResult.No)
                {
                    _playerRepository.UpdatePlayerSquadStatus(currentP.PlayerId, "Substitute", -1);
                    LoadUserPlayersAndRestoreLineup();
                    return;
                }
                else
                {
                    _playerRepository.UpdatePlayerInjuryStatus(currentP.PlayerId, true);
                    _playerRepository.UpdatePlayerSquadStatus(currentP.PlayerId, "Pool", -1);
                    LoadUserPlayersAndRestoreLineup();
                    return;
                }
            }

            if (selectedSlot != null && selectedSlot != clickedSlot)
            {
                var p1 = selectedSlot.Tag as PlayerDto;
                var p2 = clickedSlot.Tag as PlayerDto;

                int slot1Index = pitchSlots.IndexOf(selectedSlot);
                int slot2Index = pitchSlots.IndexOf(clickedSlot);

                if (p1 != null && p2 != null)
                {
                    _playerRepository.UpdatePlayerSquadStatus(p1.PlayerId, "FirstEleven", slot2Index);
                    _playerRepository.UpdatePlayerSquadStatus(p2.PlayerId, "FirstEleven", slot1Index);
                }
                else if (p1 != null && p2 == null)
                {
                    _playerRepository.UpdatePlayerSquadStatus(p1.PlayerId, "FirstEleven", slot2Index);
                }

                selectedSlot.FlatAppearance.BorderColor = Color.LightGray;
                selectedSlot.FlatAppearance.BorderSize = 1;
                selectedSlot = null;

                LoadUserPlayersAndRestoreLineup();
                return;
            }

            if (selectedSlot != null)
            {
                selectedSlot.FlatAppearance.BorderColor = Color.LightGray;
                selectedSlot.FlatAppearance.BorderSize = 1;
            }

            selectedSlot = clickedSlot;
            selectedSlot.FlatAppearance.BorderColor = Color.Yellow;
            selectedSlot.FlatAppearance.BorderSize = 2;
        }

        private void AssignPlayerToSelectedSlot(PlayerDto player)
        {
            if (selectedSlot == null) return;

            int targetSlotIndex = pitchSlots.IndexOf(selectedSlot);

            if (selectedSlot.Tag != null)
            {
                PlayerDto oldPlayer = selectedSlot.Tag as PlayerDto;
                _playerRepository.UpdatePlayerSquadStatus(oldPlayer.PlayerId, "Pool", -1);
            }

            _playerRepository.UpdatePlayerSquadStatus(player.PlayerId, "FirstEleven", targetSlotIndex);

            selectedSlot.FlatAppearance.BorderColor = Color.LightGray;
            selectedSlot.FlatAppearance.BorderSize = 1;
            selectedSlot = null;

            LoadUserPlayersAndRestoreLineup();
        }

        private void CmbCaptain_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (isInitializing) return;
            if (cmbCaptain.SelectedItem is KeyValuePair<int, string> selectedPair)
            {
                foreach (var p in userPlayers)
                {
                    if (p.IsCaptain) _playerRepository.UpdatePlayerRole(p.PlayerId, false, p.SetPieceRole);
                }
                _playerRepository.UpdatePlayerRole(selectedPair.Key, true, "None");
                LoadUserPlayersAndRestoreLineup();
            }
        }

        private void CmbRightCorner_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (isInitializing) return;
            if (cmbRightCorner.SelectedItem is KeyValuePair<int, string> selectedPair)
            {
                _playerRepository.UpdatePlayerRole(selectedPair.Key, false, "RightCorner");
                LoadUserPlayersAndRestoreLineup();
            }
        }

        private void CmbLeftCorner_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (isInitializing) return;
            if (cmbLeftCorner.SelectedItem is KeyValuePair<int, string> selectedPair)
            {
                _playerRepository.UpdatePlayerRole(selectedPair.Key, false, "LeftCorner");
                LoadUserPlayersAndRestoreLineup();
            }
        }

        private void CmbPenalty_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (isInitializing) return;
            if (cmbPenalty.SelectedItem is KeyValuePair<int, string> selectedPair)
            {
                _playerRepository.UpdatePlayerRole(selectedPair.Key, false, "Penalty");
                LoadUserPlayersAndRestoreLineup();
            }
        }

        private void BtnMedicalProcess_Click(object sender, EventArgs e)
        {
            var injuredPlayers = userPlayers.FindAll(x => x.IsInjured);
            if (injuredPlayers.Count == 0)
            {
                MessageBox.Show("Medikal departmanda sakat oyuncu bulunmuyor.", "Sağlık Raporu", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            foreach (var p in injuredPlayers)
            {
                var revive = MessageBox.Show($"{p.FullName} ({p.Position}) sakat durumda.\n\nBu oyuncuyu iyileştirip tekrar havuza geri döndürmek istiyor musunuz?", "Medikal Raporu", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (revive == DialogResult.Yes)
                {
                    _playerRepository.UpdatePlayerInjuryStatus(p.PlayerId, false);
                    _playerRepository.UpdatePlayerSquadStatus(p.PlayerId, "Pool", -1);
                    MessageBox.Show($"{p.FullName} iyileşti ve ana havuza geri döndürüldü!", "Geçmiş Olsun", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            LoadUserPlayersAndRestoreLineup();
        }

        // --- PROFESYONEL VE GERÇEKÇİ TAKTİK DİZİLİŞ KOORDİNATLARI ---
        private void ApplyFormation(int formationIndex)
        {
            int pw = pnlPitch.Width;
            int ph = pnlPitch.Height;

            // 0. İndeks her zaman Kalecidir (Sabit Kale Önü)
            pitchSlots[0].Location = new Point((pw / 2) - 50, ph - 55);
            if (pitchSlots[0].Tag == null) pitchSlots[0].Text = "KALECİ";

            List<Point> coords = GetFormationCoordinates(formationIndex, pw, ph);

            for (int i = 1; i < 11; i++)
            {
                if (i - 1 < coords.Count)
                {
                    pitchSlots[i].Location = coords[i - 1];
                    if (pitchSlots[i].Tag == null)
                    {
                        pitchSlots[i].Text = $"POZ {i}";
                    }
                }
            }
        }

        private List<Point> GetFormationCoordinates(int index, int w, int h)
        {
            List<Point> list = new List<Point>();

            switch (index)
            {
                case 0: // 1-4-4-2 (Klasik Dengeli)
                    // Defans 4'lü
                    list.Add(new Point(45, h - 150)); list.Add(new Point(200, h - 150)); list.Add(new Point(360, h - 150)); list.Add(new Point(515, h - 150));
                    // Orta Saha 4'lü
                    list.Add(new Point(45, h - 290)); list.Add(new Point(200, h - 290)); list.Add(new Point(360, h - 290)); list.Add(new Point(515, h - 290));
                    // Forvet 2'li
                    list.Add(new Point(200, h - 430)); list.Add(new Point(360, h - 430));
                    break;

                case 1: // 1-4-3-3 (Ofansif Kanat)
                    // Defans 4'lü
                    list.Add(new Point(45, h - 150)); list.Add(new Point(200, h - 150)); list.Add(new Point(360, h - 150)); list.Add(new Point(515, h - 150));
                    // Orta Saha 3'lü
                    list.Add(new Point(120, h - 280)); list.Add(new Point(280, h - 280)); list.Add(new Point(440, h - 280));
                    // Forvet 3'lü (LW, ST, RW)
                    list.Add(new Point(70, h - 430)); list.Add(new Point(280, h - 450)); list.Add(new Point(490, h - 430));
                    break;

                case 2: // 1-4-2-3-1 (Modern Oyun Kurucu)
                    // Defans 4'lü
                    list.Add(new Point(45, h - 150)); list.Add(new Point(200, h - 150)); list.Add(new Point(360, h - 150)); list.Add(new Point(515, h - 150));
                    // Defansif Orta Saha 2'li (CDM)
                    list.Add(new Point(200, h - 250)); list.Add(new Point(360, h - 250));
                    // Ofansif Orta Saha 3'lü (LM, CAM, RM)
                    list.Add(new Point(70, h - 350)); list.Add(new Point(280, h - 360)); list.Add(new Point(490, h - 350));
                    // Tek Forvet (ST)
                    list.Add(new Point(280, h - 460));
                    break;

                case 3: // 1-3-5-2 (Orta Saha Hakimiyeti)
                    // Defans 3'lü
                    list.Add(new Point(100, h - 150)); list.Add(new Point(280, h - 150)); list.Add(new Point(460, h - 150));
                    // Orta Saha 5'li (LWB, CM, CDM, CM, RWB)
                    list.Add(new Point(40, h - 280)); list.Add(new Point(160, h - 270)); list.Add(new Point(280, h - 290)); list.Add(new Point(400, h - 270)); list.Add(new Point(520, h - 280));
                    // Forvet 2'li
                    list.Add(new Point(200, h - 430)); list.Add(new Point(360, h - 430));
                    break;

                case 4: // 1-5-3-2 (Katı Savunma)
                    // Defans 5'li
                    list.Add(new Point(30, h - 150)); list.Add(new Point(150, h - 150)); list.Add(new Point(280, h - 150)); list.Add(new Point(410, h - 150)); list.Add(new Point(530, h - 150));
                    // Orta Saha 3'lü
                    list.Add(new Point(120, h - 290)); list.Add(new Point(280, h - 290)); list.Add(new Point(440, h - 290));
                    // Forvet 2'li
                    list.Add(new Point(200, h - 430)); list.Add(new Point(360, h - 430));
                    break;

                case 5: // 1-4-1-4-1 (Dirençli Merkez)
                    // Defans 4'lü
                    list.Add(new Point(45, h - 150)); list.Add(new Point(200, h - 150)); list.Add(new Point(360, h - 150)); list.Add(new Point(515, h - 150));
                    // Tek Çapa (CDM)
                    list.Add(new Point(280, h - 240));
                    // 4'lü Orta Saha
                    list.Add(new Point(50, h - 330)); list.Add(new Point(200, h - 330)); list.Add(new Point(360, h - 330)); list.Add(new Point(515, h - 330));
                    // Tek Forvet
                    list.Add(new Point(280, h - 450));
                    break;

                case 6: // 1-3-4-3 (Agresif Üçlü)
                    // Defans 3'lü
                    list.Add(new Point(100, h - 150)); list.Add(new Point(280, h - 150)); list.Add(new Point(460, h - 150));
                    // 4'lü Orta Saha
                    list.Add(new Point(60, h - 280)); list.Add(new Point(200, h - 280)); list.Add(new Point(360, h - 280)); list.Add(new Point(500, h - 280));
                    // Forvet 3'lü
                    list.Add(new Point(70, h - 430)); list.Add(new Point(280, h - 450)); list.Add(new Point(490, h - 430));
                    break;

                case 7: // 1-4-3-2-1 (Noel Ağacı)
                    // Defans 4'lü
                    list.Add(new Point(45, h - 150)); list.Add(new Point(200, h - 150)); list.Add(new Point(360, h - 150)); list.Add(new Point(515, h - 150));
                    // Orta Saha 3'lü
                    list.Add(new Point(120, h - 260)); list.Add(new Point(280, h - 260)); list.Add(new Point(440, h - 260));
                    // 2'li On Numara / İç Forvet
                    list.Add(new Point(200, h - 360)); list.Add(new Point(360, h - 360));
                    // Tek Forvet
                    list.Add(new Point(280, h - 460));
                    break;

                case 8: // 1-4-2-4 (Çift Forvet Baskı)
                    // Defans 4'lü
                    list.Add(new Point(45, h - 150)); list.Add(new Point(200, h - 150)); list.Add(new Point(360, h - 150)); list.Add(new Point(515, h - 150));
                    // Orta Saha 2'li
                    list.Add(new Point(200, h - 280)); list.Add(new Point(360, h - 280));
                    // 4'lü Hücum (LW, ST, ST, RW)
                    list.Add(new Point(50, h - 420)); list.Add(new Point(200, h - 440)); list.Add(new Point(360, h - 440)); list.Add(new Point(515, h - 420));
                    break;

                case 9: // 1-5-4-1 (Defansif Blok)
                    // Defans 5'li
                    list.Add(new Point(30, h - 150)); list.Add(new Point(150, h - 150)); list.Add(new Point(280, h - 150)); list.Add(new Point(410, h - 150)); list.Add(new Point(530, h - 150));
                    // 4'lü Orta Saha
                    list.Add(new Point(50, h - 300)); list.Add(new Point(200, h - 300)); list.Add(new Point(360, h - 300)); list.Add(new Point(515, h - 300));
                    // Tek Forvet
                    list.Add(new Point(280, h - 440));
                    break;

                default: // Varsayılan 4-4-2
                    list.Add(new Point(45, h - 150)); list.Add(new Point(200, h - 150)); list.Add(new Point(360, h - 150)); list.Add(new Point(515, h - 150));
                    list.Add(new Point(45, h - 290)); list.Add(new Point(200, h - 290)); list.Add(new Point(360, h - 290)); list.Add(new Point(515, h - 290));
                    list.Add(new Point(200, h - 430)); list.Add(new Point(360, h - 430));
                    break;
            }

            return list;
        }

        private void CmbFormations_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyFormation(cmbFormations.SelectedIndex);
        }

        private void PnlPitch_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            Pen whitePen = new Pen(Color.White, 2);
            Brush whiteBrush = Brushes.White;

            int w = pnlPitch.Width;
            int h = pnlPitch.Height;

            g.DrawRectangle(whitePen, 20, 20, w - 40, h - 40);
            g.DrawLine(whitePen, 20, h / 2, w - 20, h / 2);
            g.DrawEllipse(whitePen, (w / 2) - 60, (h / 2) - 60, 120, 120);
            g.FillEllipse(whiteBrush, (w / 2) - 4, (h / 2) - 4, 8, 8);

            g.DrawRectangle(whitePen, (w / 2) - 130, 20, 260, 90);
            g.DrawRectangle(whitePen, (w / 2) - 55, 20, 110, 35);
            g.DrawRectangle(whitePen, (w / 2) - 130, h - 110, 260, 90);
            g.DrawRectangle(whitePen, (w / 2) - 55, h - 55, 110, 35);

            g.DrawRectangle(whitePen, (w / 2) - 40, 10, 80, 10);
            g.DrawRectangle(whitePen, (w / 2) - 40, h - 20, 80, 10);
        }
    }
}