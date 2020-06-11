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

namespace Experiments
{
    public enum EMForceDirection
    {
        Z_Force,
        Y_Force,
        X_Force
    }

    public enum EMActuatorType
    {
        Solenoid,
        VCM,
        PMA
    };

    //------------------------------------------------------------------------------------------
    // 측정조건에 대해 Node 들을 만들고 성능결과를 얻고 싶을 때 개발자에게 측정 조건의 입력을 요청한다 
    //------------------------------------------------------------------------------------------
    public class CExperiment : CNode
    {
        [DisplayNameAttribute("Mesh Size [%]"), CategoryAttribute("Condition Fields"), DescriptionAttribute("Mesh Size / Shape Length * 100")]
        public double MeshSizePercent { get; set; }

        [DisplayNameAttribute("Actuator Type"), CategoryAttribute("Condition Fields"), DescriptionAttribute("Actuator Type")]
        public EMActuatorType ActuatorType { get; set; }

        public CExperiment()
        {
            // MeshSizePercent 와 Type 이 없는 이전 파일버전 인 경우는 아래의 값으로 초기화된다.
            MeshSizePercent = 7;
            ActuatorType = EMActuatorType.Solenoid;
        }

    }

    public class CForceExperiment : CExperiment
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

        [DisplayNameAttribute("Y Movement [mm]"), CategoryAttribute("\tInitial Position Fields"), DescriptionAttribute("Y Displacement")]
        public double MovingY { get; set; }

        [DisplayNameAttribute("X Movement [mm]"), CategoryAttribute("\tInitial Position Fields"), DescriptionAttribute("X Displacement")]
        public double MovingX { get; set; }

        [DisplayNameAttribute("Z Movement [mm]"), CategoryAttribute("\tInitial Position Fields"), DescriptionAttribute("Z Displacement")]
        public double MovingZ { get; set; }

        public CForceExperiment()
        {
            m_kindKey = EMKind.FORCE_EXPERIMENT;
        }

        // 파일스트림 객체에 코일 정보를 기록한다.
        // override 를 꼭 사용해야 가상함수가 아니라 현 함수가 호출된다.
        public override bool writeObject(StreamWriter writeStream)
        {
            try
            {
                CWriteFile writeFile = new CWriteFile();

                writeFile.writeBeginLine(writeStream, "ForceExperiment", 2);

                // CNode
                writeFile.writeDataLine(writeStream, "NodeName", NodeName, 3);
                writeFile.writeDataLine(writeStream, "KindKey", m_kindKey, 3);

                // CExperiment
                writeFile.writeDataLine(writeStream, "MeshSizePercent", MeshSizePercent, 3);
                writeFile.writeDataLine(writeStream, "ActuatorType", ActuatorType, 3);

                // CForceExperiment
                writeFile.writeDataLine(writeStream, "Voltage", Voltage, 3);
                writeFile.writeDataLine(writeStream, "Current", Current, 3); 
                writeFile.writeDataLine(writeStream, "MovingY", MovingY, 3);
                writeFile.writeDataLine(writeStream, "MovingX", MovingX, 3);
                writeFile.writeDataLine(writeStream, "MovingZ", MovingZ, 3);

                writeFile.writeEndLine(writeStream, "ForceExperiment", 2);
            }
            catch (Exception ex)
            {
                CNotice.printTrace(ex.Message);
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
                            m_kindKey = (EMKind)Enum.Parse(typeof(EMKind), arrayString[1]);
                            break;

                        // CExperiment
                        case "MeshSizePercent":
                            MeshSizePercent = Convert.ToDouble(arrayString[1]);
                            break;

                        case "ActuatorType":
                            ActuatorType = (EMActuatorType)Enum.Parse(typeof(EMActuatorType), arrayString[1]);
                            break;

                        // CForceExperiment
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

                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                CNotice.printTrace(ex.Message);
                return false;
            }

            return true;
        }

        public CForceExperiment Clone()
        {
            CForceExperiment forceExperiment = new CForceExperiment();

            forceExperiment.m_kindKey = this.m_kindKey;
            forceExperiment.Current = this.Current;
            forceExperiment.MovingY = this.MovingY;
            forceExperiment.MovingX = this.MovingX;
            forceExperiment.MovingZ = this.MovingZ;
            forceExperiment.NodeName = this.NodeName;
            forceExperiment.Voltage = this.Voltage;
            forceExperiment.MeshSizePercent = this.MeshSizePercent;
            forceExperiment.ActuatorType = this.ActuatorType;

            return forceExperiment;
        }
    }
}
