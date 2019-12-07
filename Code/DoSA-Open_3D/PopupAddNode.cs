using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

// 예외 처리
using System.IO;

// Debugging
using System.Diagnostics;

using Parts;
using Experiments;
using Nodes;

using gtLibrary;

namespace DoSA
{
    public partial class PopupAddNode : Form
    {
        public string NodeName { get; set; }
        bool m_bShapeNode;


        public PopupAddNode(EMKind kind, List<string> listRemainedShapeName)
        {
            InitializeComponent();

            // 추가 폼의 제목줄 변경
            switch (kind)
            {
                case EMKind.COIL:
                    this.Text = "Select a Coil Part";
                    break;

                case EMKind.MAGNET:
                    this.Text = "Select a Magnet Part";
                    break;

                case EMKind.STEEL:
                    this.Text = "Select a Steel Part";
                    break;

                case EMKind.FORCE_EXPERIMENT:
                    this.Text = "Input a Force Experiment Name";
                    break;

                case EMKind.SHOW_LIST:
                    this.Text = "Check the Part Names";
                    break;                    

                default:
                    break;
            }

            if (kind != EMKind.SHOW_LIST)
            {
                groupBoxCheckPartsName.Visible = false;
                labelCheckPartsName1.Visible = false;
                labelCheckPartsName2.Visible = false;
            }
           
            // 형상 Node 이면 콤보박스를 활성 시킨다.
            if (kind == EMKind.COIL || kind == EMKind.MAGNET || kind == EMKind.STEEL || kind == EMKind.SHOW_LIST)
            {
                groupBoxExperiments.Enabled = false;
                textBoxNodeName.Enabled = false;

                listViewNodeName.Enabled = true;

                ImageList imgListSize = new ImageList();
                imgListSize.ImageSize = new Size(1, 15);
                listViewNodeName.SmallImageList = imgListSize;

                foreach (string shapeName in listRemainedShapeName)
                {
                    string[] arrayString = { shapeName };

                    var lvItem = new ListViewItem(arrayString);
                    listViewNodeName.Items.Add(lvItem);
                }

                // 레코드, 행, Row 의 개수를 확인한다.
                if (listViewNodeName.Items.Count != 0)
                {
                    // 첫번째 레코드로 선택한다.
                    listViewNodeName.Items[0].Selected = true;
                }

                this.m_bShapeNode = true;
            }
            // 형상 Node 가 아니면 텍스트 박스를 활성화 시킨다
            else
            {
                groupBoxParts.Enabled = false;
                listViewNodeName.Enabled = false;

                textBoxNodeName.Enabled = true;

                switch (kind)
                {
                    case EMKind.FORCE_EXPERIMENT:
                        textBoxNodeName.Text = "Force";
                        break;

                    default:
                        break;
                }

                this.m_bShapeNode = false;
            }
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            // 형상이면 콤보박스 값을 사용한다.
            if (this.m_bShapeNode == true)
            {
                // 형상 데이터가 있는 경우만 동작시킨다.
                if (listViewNodeName.Items.Count != 0)
                {
                    // 콤보박스에 형상이 없어서 선택되지 못한 경우를 제외한다.
                    if (listViewNodeName.SelectedItems.Count != 0)
                    {
                        int nIndex = listViewNodeName.FocusedItem.Index;

                        NodeName = listViewNodeName.Items[nIndex].SubItems[0].Text; //인덱스 번호의 n번째 아이템 얻기                    
                    }
                    else
                    {
                        CNotice.noticeWarning("There is no selected part.\n선택된 파트가 존재하지 않습니다.");
                        return;
                    }
                }
                else
                {
                    CNotice.noticeWarning("There is no part to select.\n선택할 파트가 존재하지 않습니다.");
                    return;
                }

            }
            // 형상이 아니면 text 값을 사용한다.
            else
            {
                NodeName = string.Format("{0}", textBoxNodeName.Text);

                if (NodeName.Length == 0)
                {
                    CNotice.noticeWarning("You need to enter the Node Name.\n가상 실험의 이름을 입력하세요.");
                    return;
                }
            }

            this.DialogResult = DialogResult.OK;
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void textBoxNodeName_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (System.Text.Encoding.UTF8.GetByteCount(new char[] { e.KeyChar }) > 1)
            {
                e.Handled = true;
            }
            // Enter 가 들어오면 Login Button 과 동일하게 처리한다
            else if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                buttonOK.PerformClick();
            }   
        }
    }
}
