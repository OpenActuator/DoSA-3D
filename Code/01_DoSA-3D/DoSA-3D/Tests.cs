using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Properties Category
using System.ComponentModel;

using System.IO;

using Nodes;
using gtLibrary;

namespace Tests
{
    public enum EMForceDirection
    {
        Z_Force,
        Y_Force,
        X_Force
    }

    //------------------------------------------------------------------------------------------
    // 측정조건에 대해 Node 들을 만들고 성능결과를 얻고 싶을 때 개발자에게 측정 조건의 입력을 요청한다 
    //------------------------------------------------------------------------------------------
    public class CTest : CNode
    {
        [DisplayNameAttribute("Mesh Size [%]"), CategoryAttribute("Condition Fields"), DescriptionAttribute("Mesh Size = Model Length * Mesh Size [%] / 100")]
        public double MeshSizePercent { get; set; }

        public CTest()
        {
            // MeshSizePercent 와 Type 이 없는 이전 파일버전 인 경우는 아래의 값으로 초기화된다.
            MeshSizePercent = 7;
        }

    }

    public class CForceTest : CTest
    {
        private double m_dCurrent;

        [DisplayNameAttribute("Voltage [V]"), CategoryAttribute("\t\tInput Fields"), DescriptionAttribute("Input Voltage")]
        public double Voltage { get; set; }

        [DisplayNameAttribute("Max. Current [A]"), CategoryAttribute("\t\tInput Fields"), DescriptionAttribute("Maximum Input Current")]
        [ReadOnlyAttribute(true)]
        public double Current
        {
            // 소수점 5째 자리 까지만 출력한다.
            get { return Math.Round(m_dCurrent, 5); }
            set { m_dCurrent = value; }
        }

        [DisplayNameAttribute("X Movement [mm]"), CategoryAttribute("\tInitial Position Fields"), DescriptionAttribute("X Displacement")]
        public double MovingX { get; set; }

        [DisplayNameAttribute("Y Movement [mm]"), CategoryAttribute("\tInitial Position Fields"), DescriptionAttribute("Y Displacement")]
        public double MovingY { get; set; }

        [DisplayNameAttribute("Z Movement [mm]"), CategoryAttribute("\tInitial Position Fields"), DescriptionAttribute("Z Displacement")]
        public double MovingZ { get; set; }


        [DisplayNameAttribute("B Rotation Angle [°]"), CategoryAttribute("\tPost-Processing Fields"), DescriptionAttribute("B Rotation Angle")]
        public double B_RotationAngle { get; set; }

        // 제품 크기를 기준으로 해상도 등분이다.
        [DisplayNameAttribute("B Vector Resolution"), CategoryAttribute("\tPost-Processing Fields"), DescriptionAttribute("Resolution of Section B Vector")]
        public double B_VectorResolution { get; set; }

        public CForceTest()
        {
            KindKey = EMKind.FORCE_TEST;
            Voltage = 5.0;

            B_VectorResolution = 100;
            B_RotationAngle = 0;
        }

        // 파일스트림 객체에 코일 정보를 기록한다.
        // override 를 꼭 사용해야 가상함수가 아니라 현 함수가 호출된다.
        public override bool writeObject(StreamWriter writeStream, int nLevel)
        {
            try
            {
                CWriteFile writeFile = new CWriteFile();

                writeFile.writeBeginLine(writeStream, "ForceTest", nLevel);

                // CNode
                writeFile.writeDataLine(writeStream, "NodeName", NodeName, nLevel + 1);
                writeFile.writeDataLine(writeStream, "KindKey", KindKey, nLevel + 1);

                // CTest
                writeFile.writeDataLine(writeStream, "MeshSizePercent", MeshSizePercent, nLevel + 1);

                // CForceTest
                writeFile.writeDataLine(writeStream, "Voltage", Voltage, nLevel + 1);
                writeFile.writeDataLine(writeStream, "Current", Current, nLevel + 1); 
                writeFile.writeDataLine(writeStream, "MovingY", MovingY, nLevel + 1);
                writeFile.writeDataLine(writeStream, "MovingX", MovingX, nLevel + 1);
                writeFile.writeDataLine(writeStream, "MovingZ", MovingZ, nLevel + 1);
                writeFile.writeDataLine(writeStream, "B_RotationAngle", B_RotationAngle, nLevel + 1);
                writeFile.writeDataLine(writeStream, "B_VectorResolution", B_VectorResolution, nLevel + 1);                

                writeFile.writeEndLine(writeStream, "ForceTest", nLevel);
            }
            catch (Exception ex)
            {
                CNotice.printLog(ex.Message);

                return false;
            }

            return true;
        }

        // 코일에 대한 문자열 라인을 넘겨 받아서 코일 객체를 초기화 한다.
        public bool readObject(List<string> listStringLines)
        {
            string strTemp;
            string[] arrayString;

            try
            {
                foreach (string strLine in listStringLines)
                {
                    strTemp = strLine.Trim('\t');

                    arrayString = strTemp.Split('=');

                    if (arrayString.Length != 2)
                    {
                        CNotice.noticeWarningID("TIAP3");
                        return false;
                    }

                    switch (arrayString[0])
                    {
                        // CNode
                        case "NodeName":
                            NodeName = arrayString[1];
                            break;
                            
                        case "KindKey":
                            // 하위 버전 호환 유지 ver(0.9.13.3)
                            if (arrayString[1] == "FORCE_EXPERIMENT")
                                arrayString[1] = "FORCE_TEST";

                            KindKey = (EMKind)Enum.Parse(typeof(EMKind), arrayString[1]);
                            break;

                        // CTest
                        case "MeshSizePercent":
                            MeshSizePercent = Convert.ToDouble(arrayString[1]);
                            break;

                        // CForceTest
                        case "Voltage":
                            Voltage = Convert.ToDouble(arrayString[1]);
                            break;

                        case "Current":
                            Current = Convert.ToDouble(arrayString[1]);
                            break;
                        
                        case "MovingY":
                            MovingY = Convert.ToDouble(arrayString[1]);
                            break;

                        case "MovingX":
                            MovingX = Convert.ToDouble(arrayString[1]);
                            break;

                        case "MovingZ":
                            MovingZ = Convert.ToDouble(arrayString[1]);
                            break;

                        case "B_RotationAngle":
                            B_RotationAngle = Convert.ToDouble(arrayString[1]);
                            break;

                        case "B_VectorResolution":
                            B_VectorResolution = Convert.ToDouble(arrayString[1]);
                            break;

                        default:
                            // 해당사항이 없는 항목은 아무것도 하지 않는다. foreach 가 동작하기 때문에 return 해서는 않된다.
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                CNotice.printLog(ex.Message);

                return false;
            }

            return true;
        }

        public CForceTest Clone()
        {
            CForceTest forceTest = new CForceTest();

            forceTest.KindKey = this.KindKey;
            forceTest.Current = this.Current;
            forceTest.MovingY = this.MovingY;
            forceTest.MovingX = this.MovingX;
            forceTest.MovingZ = this.MovingZ;
            forceTest.NodeName = this.NodeName;
            forceTest.Voltage = this.Voltage;
            forceTest.MeshSizePercent = this.MeshSizePercent;

            return forceTest;
        }
    }
}
