using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Properties Category
using System.ComponentModel;

using gtLibrary;
using Parts;
using System.IO;
using System.Windows.Forms;
using DoSA;
using Onelab;

namespace Nodes
{
    // Enum 사용목적
    // - Treeview ImageList 의 인덱스
    // - CNode 에서 상속받은 객체들의 종류 구분
    public enum EMKind
    {
        PARTS,
        COIL,
        MAGNET,
        STEEL,
        TESTS,
        FORCE_TEST,
        SHOW_LIST
    };

    public class CNode
    {
        public EMKind m_kindKey;

        private string m_nodeName;

        ///  \t\t\t 갯수가 많을수록 해당 카테고리가 상측으로 올라간다.
        [DisplayNameAttribute("Node Name"), CategoryAttribute("\t\t\tCommon Fields"), DescriptionAttribute("Part or Test name")]
        public string NodeName
        {
            get { return m_nodeName; }
            set { m_nodeName = value; }
        }

        // 작업파일 저장을 각 객체에서 진행하고 있다.
        public virtual bool writeObject(StreamWriter writeFile, int nLevel) { return true; }
        public virtual bool readObject(List<string> listStringLines, ref CNode node) { return true; }
    }

    public class CDesign
    {
        // Design 객체의 이름
        public string m_strDesignName;

        // Design 객체의 객체 디렉토리
        // Design 디렉토리는 복사가 가능하기 때문에 아래의 디렉토리는 저장하지는 않는다
        public string m_strDesignDirPath;

        public bool m_bChanged;

        // Design 에 사용되는 부품이나 시험조건을 저장하는 List 이다.
        private List<CNode> m_listNode = new List<CNode>();

        COnelab onelab = new COnelab();

        private double m_dMinX, m_dMaxX;
        private double m_dMinY, m_dMaxY;
        private double m_dMinZ, m_dMaxZ;

        private double m_dShapeVolumeSize;
        
        public double MinX { get { return m_dMinX; } set { m_dMinX = value; } }
        public double MaxX { get { return m_dMaxX; } set { m_dMaxX = value; } }
        public double MinY { get { return m_dMinY; } set { m_dMinY = value; } }
        public double MaxY { get { return m_dMaxY; } set { m_dMaxY = value; } }
        public double MinZ { get { return m_dMinZ; } set { m_dMinZ = value; } }
        public double MaxZ { get { return m_dMaxZ; } set { m_dMaxZ = value; } }

        public double ShapeVolumeSize { get { return m_dShapeVolumeSize; } set { m_dShapeVolumeSize = value; } }

        // Get 전용로 오픈한다
        public List<CNode> NodeList
        {
            get
            {
                return m_listNode;
            }
        }
                
        public bool calcShapeSize(string strMeshFileFullName)
        {
            if (false == onelab.calcShapeSize(strMeshFileFullName))
                return false;

            m_dMinX = onelab.MinX;
            m_dMaxX = onelab.MaxX;

            m_dMinY = onelab.MinY;
            m_dMaxY = onelab.MaxY;

            m_dMinZ = onelab.MinZ;
            m_dMaxZ = onelab.MaxZ;

            m_dShapeVolumeSize = Math.Abs(m_dMaxX - m_dMinX) * Math.Abs(m_dMaxY - m_dMinY) * Math.Abs(m_dMaxZ - m_dMinZ);

            return true;
        }                           

        // 모든 형상명 (읽어드린 후 변경 없음)
        private List<string> m_listAllShapeName = new List<string>();

        public List<string> AllShapeNameList
        {
            get
            {
                return m_listAllShapeName;
            }
            set
            {
                m_listAllShapeName = value;
            }
        }

        // 형상 중에 사용하고 남아 있는 형상이름 목록
        private List<string> m_listRemainedShapeName = new List<string>();

        public List<string> RemainedShapeNameList
        {
            get
            {
                return m_listRemainedShapeName;
            }
            set
            {
                m_listRemainedShapeName = value;
            }
        }      

        public CDesign()
        {
            // 꼭 초기화가 되어야 한다.
            m_strDesignName = string.Empty;
            m_strDesignDirPath = string.Empty;

            m_bChanged = false;
        }

        public CNode getNode(string nodeName)
        {
            foreach (CNode node in m_listNode)
            {
                // 같은 이름의 노드가 있으면 리턴한다.
                if (node.NodeName == nodeName)
                    return node;
            }

            return null;
        }

        // 신규로 사용이 가능한 이름인가 검사함
        public bool enableUseNodeName(string nodeName)
        {
            bool bEnable = true;

            // 추가된 Node 중에 있으면 안된다.
            if (this.isExistNode(nodeName) == true)
                bEnable = false;

            // 남아있는 Shape 이름 중에도 있으면 안된다.
            if (m_listRemainedShapeName.Contains(nodeName) == true)
                bEnable = false;

            return bEnable;
        }

        public double getShapeWidthX()
        {
            return Math.Abs(m_dMaxX - m_dMinX);
        }

        public double getShapeWidthY()
        {
            return Math.Abs(m_dMaxY - m_dMinY);
        }

        public double getShapeWidthZ()
        {
            return Math.Abs(m_dMaxZ - m_dMinZ);
        }

        public bool getShapeMinaxX(ref double dMinX, ref double dMaxX)
        {
            dMinX = m_dMinX;
            dMaxX = m_dMaxX;

            return true;
        }

        public bool getShapeMinMaxY(ref double dMinY, ref double dMaxY)
        {
            dMinY = m_dMinY;
            dMaxY = m_dMaxY;

            return true;
        }

        public bool getShapeMinMaxZ(ref double dMinZ, ref double dMaxZ)
        {
            dMinZ = m_dMinZ;
            dMaxZ = m_dMaxZ;

            return true;
        }

        public int getNodeSize()
        {
            return m_listNode.Count;
        }

        public bool addNode(CNode node)
        {
            if (isExistNode(node.NodeName) == true)
                return false;

            string nodeName = node.NodeName;
            EMKind kind = node.m_kindKey;

            switch (kind)
            {
                // 형상 파트를 추가하는 경우
                case EMKind.COIL:
                case EMKind.MAGNET:
                case EMKind.STEEL:
                    // 남아있는 이름 리스트에 없으면 추가작업을 하지 않는다
                    if (m_listRemainedShapeName.Contains(nodeName) == false)
                    {
                        CNotice.printLog("형상이 아닌 node 을 형상 객체처리를 하려고 합니다.");
                        return false;
                    }

                    // Shape 명을 제거한다
                    m_listRemainedShapeName.Remove(nodeName);
                    break;

                // 시험 노드를 추가하는 경우
                default:
                    // 남아있는 Shape 형상 명으로 예약되었거나 사용중이 이름이라면 추가 작업을 취소한다.
                    if (enableUseNodeName(nodeName) == false)
                    {
                        if (CSettingData.m_emLanguage == EMLanguage.Korean)
                            CNotice.noticeWarning("동일한 이름의 가상실험이 이미 존재 합니다.");
                        else
                            CNotice.noticeWarning("There is the same name test already.");

                        return false;
                    }                        

                    break;
            }

            m_listNode.Add(node);

            return true;
        }

        // 같이 이름의 Node 가 있는지 검사한다.
        public bool isExistNode(string nodeName)
        {
            foreach (CNode node in m_listNode)
            {
                if (node.NodeName == nodeName)
                    return true;
            }

            return false;
        }

        public bool deleteNode(string nodeName)
        {
            EMKind kind;
            string strName;

            // 추가된 Node 중에 없으면 바로 리턴한다.
            if (isExistNode(nodeName) == false)
                return false;

            foreach (CNode node in m_listNode)
            {
                if (node.NodeName == nodeName)
                {
                    kind = node.m_kindKey;
                    strName = node.NodeName;

                    // 자기회로 부품과 특수 형상인 경우만 다시 살려낸다
                    if (kind == EMKind.COIL || kind == EMKind.MAGNET || kind == EMKind.STEEL)
                    {
                        // Shape 를 사용하지 않게 되었으므로 다시 추가한다
                        m_listRemainedShapeName.Add(strName);
                    }

                    // 삭제 후 바로 빠져나가야 한다.
                    m_listNode.Remove(node);
                    return true;
                }
            }

            return false;
        }

        public void clearDesign()
        {
            m_strDesignName = string.Empty;
            m_strDesignDirPath = string.Empty;

            m_listAllShapeName.Clear();
            m_listRemainedShapeName.Clear();

            m_listNode.Clear();
        }

        // 해당 종류의 노드 갯수를 얻어 온다
        public int getKindNodeSize(EMKind kind)
        {
            int size = 0;

            foreach (CNode node in m_listNode)
            {
                if (node.m_kindKey == kind)
                    size++;
            }

            return size;
        }

        // 이동 파트 갯수를 얻어 온다
        public int getMovingPartSize()
        {
            int nMovingPartCount = 0;

            foreach (CNode node in m_listNode)
            {
                if (node.GetType().BaseType.Name == "CParts")
                    if (((CParts)node).MovingPart == EMMoving.MOVING)
                        nMovingPartCount ++;
            }

            return nMovingPartCount;
        }

        public void writeObject(StreamWriter writeStream, int nLevel)
        {
            CWriteFile writeFile = new CWriteFile();
            string strTemp = string.Empty;

            writeFile.writeBeginLine(writeStream, "Design", nLevel);
            writeFile.writeDataLine(writeStream, "DesignName", m_strDesignName, nLevel + 1);

            foreach (string str in m_listAllShapeName)
                strTemp = strTemp + str + ',';
            writeFile.writeDataLine(writeStream, "AllShapeName", strTemp, nLevel + 1);

            strTemp = string.Empty;

            foreach (string str in m_listRemainedShapeName)
                strTemp = strTemp + str + ',';
            writeFile.writeDataLine(writeStream, "RemainedShapeName", strTemp, nLevel + 1);

            writeFile.writeDataLine(writeStream, "ShapeMinX", m_dMinX.ToString(), nLevel + 1);
            writeFile.writeDataLine(writeStream, "ShapeMaxX", m_dMaxX.ToString(), nLevel + 1);

            writeFile.writeDataLine(writeStream, "ShapeMinY", m_dMinY.ToString(), nLevel + 1);
            writeFile.writeDataLine(writeStream, "ShapeMaxY", m_dMaxY.ToString(), nLevel + 1);

            writeFile.writeDataLine(writeStream, "ShapeMinZ", m_dMinZ.ToString(), nLevel + 1);
            writeFile.writeDataLine(writeStream, "ShapeMaxZ", m_dMaxZ.ToString(), nLevel + 1);

            foreach (CNode node in NodeList)
            {
                node.writeObject(writeStream, nLevel);
            }

            writeFile.writeEndLine(writeStream, "Design", nLevel);
        }

        public void getMaterial()
        {
            List<string> listMaterial = new List<string>();
            string strMaterial = "Air";
            CParts nodeParts = null;

            bool bCheck;

            //femm.getMaterial(strMaterial);
            listMaterial.Add(strMaterial);

            foreach (CNode node in NodeList)
            {
                bCheck = false;
                if (node.GetType().BaseType.Name == "CParts")
                {
                    nodeParts = (CParts)node;
                    strMaterial = nodeParts.getMaterial();

                    /// 현 파트의 재료가 기존에 저장된 Material 과 겹치는지를 확인한다.
                    foreach (string strTemp in listMaterial)
                        if (strTemp == strMaterial)
                            bCheck = true;

                    // 겹치지 않는 재료만 추가한다.
                    if (bCheck == false)
                    {
                        listMaterial.Add(strMaterial);
                        //femm.getMaterial(nodeParts.getMaterial());
                    }

                }
            }
        }

        /// <summary>
        /// 이름이 겹치는 Node 가 있는지를 확인한다.
        /// 
        /// [목적]
        ///  - PropertyGrid 에서 이름을 수정할 때 이름이 겹치는 문제를 해결하기 위해 추가함
        ///  - 이름을 수정 할 때 기존에 동일한 이름이 있는지를 확인하고 이름을 변경하는 것이 좋으나,
        ///    PropertyGrid 에서 이름을 변경하면 바로 NodeList 의 이름이 바뀌기 때문에 
        ///    이름 겹침으로 수정이 잘못되었음을 확인하고 이름을 복원하는 방법을 사용해 해결함
        /// </summary>
        /// <returns>이름이 겹치면 true 리턴</returns>
        public bool duplicateNodeName()
        {
            /// 비교의 앞 이름은 m_listNode.Count - 1 까지 이다.
            for (int i = 0; i < m_listNode.Count - 1; i++)
            {
                /// 비교의 뒤 이름은 1 부터 이다.
                for (int j = i + 1; j < m_listNode.Count; j++)
                {
                    if (m_listNode[i].NodeName == m_listNode[j].NodeName)
                        return true;
                }
            }

            return false;
        }

        public bool isExistMagnet()
        {
            foreach (CNode node in m_listNode)
            {
                if (node.m_kindKey == EMKind.MAGNET)
                    return true;
            }

            return false;
        }

        internal bool isCoilAreaOK()
        {
            bool ret = false;

            foreach (CNode node in m_listNode)
            {
                if (node.m_kindKey == EMKind.COIL)
                {
                    if (((CCoil)node).InnerDiameter > 0 && ((CCoil)node).OuterDiameter > 0
                        && ((CCoil)node).Height > 0 && ((CCoil)node).CopperDiameter > 0)
                        ret = true;
                }
            }

            return ret;
        }

        internal bool isCoilSpecificationOK()
        {
            bool ret = false;

            foreach (CNode node in m_listNode)
            {
                if (node.m_kindKey == EMKind.COIL)
                {
                    if (((CCoil)node).Resistance > 0 && ((CCoil)node).Turns > 0)
                        ret = true;
                }
            }

            return ret;
        }


        //public bool isDesignShapeOK(double dStroke = 0)
        //{
        //    CFace face = null;
        //    bool bError = false;
        //    CParts nodeParts = null;

        //    // Moving Part 를 Stroke 만큼 이동시킨다.
        //    foreach (CNode node in NodeList)
        //    {
        //        if (node.GetType().BaseType.Name == "CParts")
        //        {
        //            nodeParts = (CParts)node;

        //            if (nodeParts.MovingPart == EMMoving.MOVING)
        //            {
        //                face = nodeParts.Face;
        //                face.BasePoint.m_dY = face.BasePoint.m_dY + dStroke;
        //            }
        //        }
        //    }

        //    if (isIntersectedAllLines() == true)
        //    {
        //        CNotice.noticeWarningID("LCBP");
        //        bError = true;
        //    }

        //    if (isContactedMovingParts() == true)
        //    {
        //        CNotice.noticeWarningID("IHOT");
        //        bError = true;
        //    }

        //    // Moving Part 를 Stroke 만큼 복원 시킨다.
        //    foreach (CNode node in NodeList)
        //    {
        //        if (node.GetType().BaseType.Name == "CParts")
        //        {
        //            nodeParts = (CParts)node;

        //            if (nodeParts.MovingPart == EMMoving.MOVING)
        //            {
        //                face = nodeParts.Face;
        //                face.BasePoint.m_dY = face.BasePoint.m_dY - dStroke;
        //            }
        //        }
        //    }

        //    if (bError == true)
        //        return false;
        //    else
        //        return true;

        //    return true;
        //}
    }
}
