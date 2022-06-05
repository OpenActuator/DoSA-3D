using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Debugging
using System.Diagnostics;

// 파일처리
using System.IO;

// Sleep 함수 사용
using System.Threading;

// WINAPI 사용
using System.Runtime.InteropServices;

using Parts;
using Tests;
using Nodes;

using gtLibrary;
//using DoSA;
using System.Windows.Forms;
using System.Resources;

namespace Onelab
{
    public enum EMTestType
    {
        FORCE_TEST,
        STROKE_TEST,
        CURRENT_TEST,
        MOVEMENT_TEST
    }

    public class COnelab
    {
        private double m_dMinX = 0, m_dMaxX = 0;
        private double m_dMinY = 0, m_dMaxY = 0;
        private double m_dMinZ = 0, m_dMaxZ = 0;

        public double MinX { get { return m_dMinX; } }
        public double MaxX { get { return m_dMaxX; } }
        public double MinY { get { return m_dMinY; } }
        public double MaxY { get { return m_dMaxY; } }
        public double MinZ { get { return m_dMinZ; } }
        public double MaxZ { get { return m_dMaxZ; } }

        protected CReadFile m_readFile = new CReadFile();

        public bool calcShapeSize(string strMeshFileFuleName)
        {
            List<double> listDataX = new List<double>();   
            List<double> listDataY = new List<double>();   
            List<double> listDataZ = new List<double>();

            try
            {
                getMeshNodeCoordinate(strMeshFileFuleName, ref listDataX, ref listDataY, ref listDataZ);

                m_dMinX = listDataX.Min();
                m_dMaxX = listDataX.Max();
                m_dMinY = listDataY.Min();
                m_dMaxY = listDataY.Max();
                m_dMinZ = listDataZ.Min();
                m_dMaxZ = listDataZ.Max();
            
            }
            catch (Exception ex)
            {
                CNotice.printLog(ex.Message);
                return false;
            }

            return true;        

        }

        private bool getMeshNodeCoordinate(string strMeshFileFullName, ref List<double> listDataX, ref List<double> listDataY, ref List<double> listDataZ)
        {

            CReadFile readFile = new CReadFile();

            List<string> listBlockLines = new List<string>();
            List<double> listColumnData = new List<double>();   

            try
            {
                readFile.readBlock(strMeshFileFullName, ref listBlockLines, "$Nodes", "$EndNodes");

                foreach(string strLine in listBlockLines)
                {
                    CParsing.getDataInAllLine(strLine, ref listColumnData, ' ');

                    // 3개의 데이터일때만 좌표 데이터 이다.
                    if(listColumnData.Count == 3)
                    {
                        listDataX.Add(listColumnData[0]);
                        listDataY.Add(listColumnData[1]);
                        listDataZ.Add(listColumnData[2]);
                    }

                    listColumnData.Clear();
                }
            }
            catch (Exception ex)
            {
                CNotice.printLog(ex.Message);
                return false;
            }

            return true;        
        }

    }


    public class CScript
    {
        //-----------------------------------------------------------------------------
        // API 함수 사용
        //-----------------------------------------------------------------------------
        // [주의사항] 꼭 Class 안에 존재해야 함

        [DllImport("user32.dll", EntryPoint = "MoveWindow")]
        private static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        [DllImport("user32.dll")]
        private static extern Boolean ShowWindow(IntPtr hWnd, Int32 nCmdShow);

        //-----------------------------------------------------------------------------

        private static System.Diagnostics.Process m_process;

        protected CWriteFile m_writeFile = new CWriteFile();

        public static void runScript(string strCmd, string strArgs, bool bWaiting = false, ProgressBar progressBar = null)
        {
            //------------------------------------------------------------
            // runProcess 호출 예제
            //------------------------------------------------------------

            //// Process 의 Arguments 에서 스페이스 문제가 발생한다.
            //// 아래와 같이 묶음처리를 사용한다.
            //string strArguments = " " + m_manageFile.solveDirectoryNameInPC(strRunScriptFileFullName);

            //runScript(strFileFullName, strArguments, m_strRunScriptFileFullName, true, progressBarMovement);

            //progressBarMovement.Value = progressBarTime.Maximum;
            //------------------------------------------------------------

            try
            {
                m_process = new System.Diagnostics.Process();
                m_process.StartInfo.FileName = strCmd;
                m_process.StartInfo.Arguments = strArgs;

                m_process.Start();

                // 프로세스를 기다리게 설정된 경우만 사용된다.
                if (bWaiting == true)
                {
                    // ProgressBar 동작없이 기다리는 프로세스
                    if (progressBar == null)
                    {
                        // 작업을 마칠때 까지 기다린다
                        m_process.WaitForExit();

                        // 중간 중간에 이벤트를 받을 수 있도록 한다
                        System.Windows.Forms.Application.DoEvents();
                    }
                    // ProgressBar 동작을 하며 기다리는 프로세스
                    else
                    {
                        while (!m_process.HasExited)
                        {
                            progressBar.PerformStep();

                            if (progressBar.Value == progressBar.Maximum)
                                progressBar.Value = 0;

                            Thread.Sleep(500);
                        }
                    }
                }

                // 약간의 지연시간이 있어야 창크기의 조절이 가능하다 
                //Thread.Sleep(100);
                //MoveWindow(m_process.MainWindowHandle, 300, 300, 1024, 768, true);

                //const int SW_MAXIMIZE = 3;
                //ShowWindow(m_process.MainWindowHandle, SW_MAXIMIZE);

            }
            catch (Exception ex)
            {
                CNotice.printLog(ex.Message);
            }
        }

        public static void moveGmshWindow(int iPosX, int iPosY, int iSizeX = 1024, int iSizeY = 768)
        {
            Process[] processList = Process.GetProcessesByName("gmsh");

            if (processList.Length > 1)
            {
                CNotice.noticeWarningID("OOFP");
                return;
            }

            if (processList.Length != 1)
                return;

            Thread.Sleep(100);
            MoveWindow(m_process.MainWindowHandle, iPosX, iPosY, iSizeX, iSizeY, true);
        }
    }

    /// <summary>
    /// # Script 생성기에서 주석처리는 첫번째 자리에 # 이 위치할 경우이다. (GetDP 주석은 // 이다.)
    /// # Script 명령어에서 { 기호를 사용하는 경우 {{ 가 발생하지 않도록 주의하라  
    /// </summary>
    public class CScriptContents
    {
        //--------------------------------------------------
        // 주의사항
        //--------------------------------------------------
        // - 내부에 사용되는 { 기호와 {{}} 는 분리하여 꼭 사용하라.
        // - 메쉬 후에 파트명을 읽어낸다. (파트명 파일의 생성 유무를 사용해서 Gmsh 자동동작을 기다리기 때문이다.

        #region ---------------------------- 01_Read Part Name ----------------------------

        public string m_str01_CheckSTEP_Script =
        @"#CHECK_STEP,2
# 1 : Step File Full Name
# 2 : Part Name File Full Name
# 3 : Design Name

# Script 생성기에서 주석처리는 첫번째 자리에 # 이 위치할 경우이다. (GetDP 주석은 // 이다.)
# Script 명령어에서 { 기호를 사용하는 경우 {{ 가 발생하지 않도록 주의하라  

SetFactory(""OpenCASCADE"");

Merge ""{{1}}"";

STEP_Volumes[] = Volume '*';

For k In {0 : #STEP_Volumes[]-1}

    stepName = StrReplace(Volume{STEP_Volumes[k]}, ""Shapes/"", """");

    Printf(stepName) >> ""{{2}}"";

EndFor

Geometry.Points = 0;
Geometry.Curves = 0;
Geometry.Surfaces = 1;
Geometry.Volumes = 0;
Geometry.VolumeLabels = 1;

Geometry.SurfaceType = 2;

Mesh.SurfaceEdges = 0;
Mesh.VolumeEdges = 0;

# 해석목적이 아니라 제품 크기를 계산하기 위해 2D Mesh 진행하고 파일을 보관한다.
Mesh 2;

Save ""{{3}}.msh"";

";
        #endregion

        #region ---------------------------- 02 Show Shape  ----------------------------

        public string m_str02_Show_Shape_Script =
        @"#CHECK_STEP,1
# 1 : Step File Full Name
# 2 : Moving Part Index Number
# 3 : Moving X
# 4 : Moving Y
# 5 : Moving Z

mm = 1e-3;

SetFactory(""OpenCASCADE"");

Merge ""{{1}}"";

STEP_Volumes[] = Volume '*';

Dilate { {0, 0, 0}, {mm, mm, mm} } { Volume{STEP_Volumes[]}; }

volMovingPart = STEP_Volumes[{{2}}];

Translate { {{3}} * mm , {{4}} * mm, {{5}} * mm } { Volume{ volMovingPart }; }

Geometry.Points = 0;
Geometry.Curves = 0;
Geometry.Surfaces = 1;
Geometry.Volumes = 0;
Geometry.VolumeLabels = 1;

Geometry.SurfaceType = 2;

Mesh.SurfaceEdges = 0;
Mesh.VolumeEdges = 0;

";
        #endregion


        #region ---------------------------- 11_Define.geo ----------------------------

        ///----------------------------------------------
        /// Define 번호 규칙
        ///----------------------------------------------
        /// 
        /// Volume Objects    : 1 ~ 199
        ///  - 1 ~ 198 : Parts
        ///  - 199 : Air (Outer, Inner 같이 사용)
        ///  
        /// Skin Objects      : 301 ~ 399
        ///  - 301 ~ 398 : Parts Skin
        ///  - 399 : Air Skin (Outer Box 만)
        ///  
        public string m_str11_Define_Script =
        @"#DEFINE,0

mm = 1e-3;

AIR = 199;
SKIN_AIR = 399;

SKIN_MOVING = 301;
SKIN_STEEL = 302;

";
        #endregion

        #region ---------------------------- 12_BH.pro ----------------------------

        public bool getScriptBH(string strMaterialName, ref string strScriptBH, List<double> listH, List<double> listB)
        {
            string strB, strH;

            strB = "    Mat_" + strMaterialName + "_B() = {\n    ";

            foreach (double dB in listB)
                strB += dB.ToString() + ", ";

            // 마지막 ", " 를 제거한다.
            int nIndex = strB.Length - 2;
            strB = strB.Remove(nIndex);

            strB += "};\n\n";


            strH = "    Mat_" + strMaterialName + "_H() = {\n    ";

            foreach (double dH in listH)
                strH += dH.ToString() + ", ";

            // 마지막 ", " 를 제거한다.
            nIndex = strH.Length - 2;
            strH = strH.Remove(nIndex);

            strH += "};";

            strScriptBH = strB + strH;

            return true;
        }
        
        public string m_str12_BH_Calulate_Script =
        @"#BH,1
# 1 : Material Name

# Script 생성기에서 주석처리는 첫번째 자리에 # 이 위치할 경우이다. (GetDP 주석은 // 이다.)
# Script 명령어에서 { 기호를 사용하는 경우 {{ 가 발생하지 않도록 주의하라  

Mat_{{1}}_B2() = Mat_{{1}}_B()^2;
Mat_{{1}}_nu() = Mat_{{1}}_H() / Mat_{{1}}_B();
Mat_{{1}}_nu(0) = Mat_{{1}}_nu(1);

Mat_{{1}}_nu_B2() = ListAlt[Mat_{{1}}_B2(), Mat_{{1}}_nu()];
nu_{{1}}[] = InterpolationLinear[SquNorm[$1]]{ Mat_{{1}}_nu_B2() };
dnudb2_{{1}}[] = dInterpolationLinear[SquNorm[$1]]{ Mat_{{1}}_nu_B2() };
H_{{1}}[] = nu_{{1}}[$1] * $1 ;
dhdb_{{1}}[] = TensorDiag[1,1,1] * nu_{{1}}[$1#1] + 2 * dnudb2_{{1}}[#1] * SquDyadicProduct[#1];
dhdb_{{1}}_NL[] = 2 * dnudb2_{{1}}[$1] * SquDyadicProduct[$1] ;

";
        #endregion

        #region ---------------------------- 21_Import STEP (*.geo) ----------------------------

        public string m_str21_Import_Script =
        @"#IMPORT,1
# 1 : STEP File Name

# Script 생성기에서 주석처리는 첫번째 자리에 # 이 위치할 경우이다. (GetDP 주석은 // 이다.)
# Script 명령어에서 { 기호를 사용하는 경우 {{ 가 발생하지 않도록 주의하라  

SetFactory(""OpenCASCADE"");

Include ""Define.geo"";

Mesh.Optimize = 1;
Mesh.VolumeEdges = 0;
Solver.AutoMesh = 2;

Merge ""{{1}}"";

STEP_Volumes[] = Volume '*';

Dilate { {0, 0, 0}, {mm, mm, mm} } { Volume{STEP_Volumes[]}; }

# Message console 을 보이게 한다. 
# 창크기는 조절이 되지만, 자동으로 Message window 를 보이는 것은 실패했다.
General.MessageHeight = 500;
General.ShowMessagesOnStartup = 1;

";
        #endregion

        #region ---------------------------- 22_Air_Region (*.geo) ----------------------------

        public string m_str22_Region_Script =
        @"#IMPORT,11
# 1 : Mesh Size
# 2 : Outer Min X
# 3 : Outer Min Y
# 4 : Outer Min Z
# 5 : Outer Width X, Y, Z
# 6 : Outer Min X
# 7 : Outer Min Y
# 8 : Outer Min Z
# 9 : Outer Width X
# 10 : Outer Width Y
# 11 : Outer Width Z

# Script 생성기에서 주석처리는 첫번째 자리에 # 이 위치할 경우이다. (GetDP 주석은 // 이다.)
# Script 명령어에서 { 기호를 사용하는 경우 {{ 가 발생하지 않도록 주의하라  

volOuterBox = newv; Box(newv) = { {{2}}*mm, {{3}}*mm, {{4}}*mm, {{5}}*mm, {{5}}*mm, {{5}}*mm };
volInnerBox = newv; Box(newv) = { {{6}}*mm, {{7}}*mm, {{8}}*mm, {{9}}*mm, {{10}}*mm, {{11}}*mm };

# volInnerBox 를 빼서 volOuterAir을 만들고, volInnerBox에 STEP_Volumes() 을 빼서 volInnerAir 만드는 순서로 작업한다.
volOuterAir = newv; BooleanDifference(newv) = { Volume{volOuterBox}; Delete; }{ Volume{volInnerBox}; };
volInnerAir = newv; BooleanDifference(newv) = { Volume{volInnerBox}; Delete; }{ Volume{STEP_Volumes()}; };

# 경계면 처리
BooleanFragments{ Volume{volOuterAir, volInnerAir}; Delete; }{}
BooleanFragments{ Volume{volInnerAir, STEP_Volumes()}; Delete; }{}

Characteristic Length { PointsOf{ Volume{volOuterAir}; } } = {{1}} * 8.0;
Characteristic Length { PointsOf{ Volume{volInnerAir}; } } = {{1}} * 2.0;
Characteristic Length { PointsOf{ Volume{STEP_Volumes[]}; } } = {{1}} * 1.0;

# 작업 검토 중
#Characteristic Length { PointsOf{ Volume{volMovingParts}; } } = {{1}} * 0.8;

Physical Surface(SKIN_MOVING) = skinMoving();
Physical Surface(SKIN_STEEL) = skinSteel();

volAll() = Volume '*';
skinAir() = CombinedBoundary{ Volume{ volAll() }; };

Physical Volume(AIR) = {volInnerAir, volOuterAir};
Physical Surface(SKIN_AIR) = skinAir();

";
        #endregion

        #region ---------------------------- 23_Group (*.pro) ----------------------------

        public string m_str23_Group_Script =
        @"#DEFINE,0
Include ""Define.geo"";
Include ""BH.pro"";

Group {
    volAir  = Region[AIR];
    skinAir = Region[SKIN_AIR];

    skinMoving = Region[SKIN_MOVING];
    skinSteel = Region[SKIN_STEEL];

";
        #endregion

        #region ---------------------------- 31_Function (*.pro) ----------------------------

        public string m_str31_Function_Script =
        @"#DEFINE,0

Function {

    mu0 = 4*Pi*1e-7;

    Nb_max_iter = 30;
    stop_criterion = 1e-5;
    relaxation_factor = 1.0;

    mu[volAir] = mu0;
    nu[volAir] = 1.0/mu0;

";
        #endregion

        #region ---------------------------- 32_Constraint (*.pro) ----------------------------

        public string m_str32_Constraint_Script =
        @"#DEFINE,0

# Script 생성기에서 주석처리는 첫번째 자리에 # 이 위치할 경우이다. (GetDP 주석은 // 이다.)
# Script 명령어에서 { 기호를 사용하는 경우 {{ 가 발생하지 않도록 주의하라  

Constraint {
    { Name cstDirichlet_A_0 ;
        Case {
            { Region skinAir ; Type Assign ; Value 0. ; }
        }
    }

    { Name cstForceMoving;
        Case {
            { Region skinMoving ; Value 1. ; }
        }
    }    
    
    { Name cstGaugeCondition_A ; Type Assign ;
        Case {
            { Region domainALL ; SubRegion skinAir ; Value 0. ; }
        }
    }    
}

FunctionSpace {
    
    { Name fsHcurl_A_3D ; Type Form1 ;
        BasisFunction {
            { Name se ; NameOfCoef ae ; Function BF_Edge ;
                Support domainALL ; Entity EdgesOf[ All ] ; }
        }
        Constraint {
            { NameOfCoef ae;    EntityType EdgesOf ; NameOfConstraint cstDirichlet_A_0 ; }
            
            { NameOfCoef ae  ;    EntityType EdgesOfTreeIn ; EntitySubType StartingOn ;
                 NameOfConstraint cstGaugeCondition_A ; }
        }
    }
    
    { Name fsForceMoving ; Type Form0 ;
        BasisFunction {
            { Name sn ; NameOfCoef un ; Function BF_GroupOfNodes ;
              Support volAir ; Entity GroupsOfNodesOf[ skinMoving ] ; }
        }
        Constraint {
            { NameOfCoef un ; EntityType GroupsOfNodesOf ; NameOfConstraint cstForceMoving ; }    
        }
    }
}

Jacobian {
    { Name jbVolume ;
        Case { 
            { Region All ;       Jacobian Vol ; }
        }
    }
}

Integration {
    { Name igElement ;
        Case {
            {    Type Gauss ;
                Case {
                    { GeoElement Triangle    ; NumberOfPoints  4 ; }
                    { GeoElement Quadrangle  ; NumberOfPoints  4 ; }
                    { GeoElement Tetrahedron ; NumberOfPoints  4 ; }
                }
            }
        }
    }
}

";
        #endregion

        #region ---------------------------- 33_Formulation_Resolution (*.pro) ----------------------------

        public string m_str33_Formulation_Resolution_Script =
        @"#DEFINE,3
# 1 : Coil Name
# 2 : First Line String of Magnet
# 3 : Second Line String of Magnet

# Script 생성기에서 주석처리는 첫번째 자리에 # 이 위치할 경우이다. (GetDP 주석은 // 이다.)
# Script 명령어에서 { 기호를 사용하는 경우 {{ 가 발생하지 않도록 주의하라  

Formulation {

    { Name fmMagStatic_A; Type FemEquation;
        Quantity {
            { Name qnt_A; Type Local; NameOfSpace fsHcurl_A_3D; }
            { Name qnt_MovingForce ; Type Local ; NameOfSpace fsForceMoving ; }
        }
        Equation {
            Integral { [ nu[{d qnt_A}] * Dof{d qnt_A} , {d qnt_A} ] ;
                In domainALL ; Jacobian jbVolume ; Integration igElement ; }
                
            Galerkin { JacNL [ dhdb_NL[{d qnt_A}] * Dof{d qnt_A} , {d qnt_A} ] ;
                In domainNL ; Jacobian jbVolume ; Integration igElement ; }

{{2}}
{{3}}
                
            Galerkin { [ -js0[], {qnt_A} ] ;
                In vol{{1}} ; Jacobian jbVolume ; Integration igElement ; }
                
            Galerkin { [ 0 * Dof{qnt_MovingForce} , {qnt_MovingForce} ] ;
                In volAir ; Jacobian jbVolume ; Integration igElement ; }

        }
    }
}

Resolution {
    { Name rsMagStatic_A ;
        System {
            { Name sys_A ; NameOfFormulation fmMagStatic_A ; }
        }

        Operation {
            IterativeLoop[Nb_max_iter, stop_criterion, relaxation_factor]{
                GenerateJac[sys_A] ; SolveJac[sys_A] ; }
            SaveSolution[sys_A] ;
            
            //PostOperation[Get_Force] ;
        }
    }
}

";
        #endregion

        #region ---------------------------- 41_PostProcessing (*.pro) ----------------------------


        public string m_str41_PostProcessing_Script =
        @"#DEFINE,1
# 1 : Coil Name

# Script 생성기에서 주석처리는 첫번째 자리에 # 이 위치할 경우이다. (GetDP 주석은 // 이다.)
# Script 명령어에서 { 기호를 사용하는 경우 {{ 가 발생하지 않도록 주의하라  

PostProcessing {

    { Name ppMagStatic_A ; NameOfFormulation fmMagStatic_A ;
        PostQuantity {
            
            { Name b ; Value { 
                Term { [ {d qnt_A} ]; In domainALL; Jacobian jbVolume; } 
                } 
            }    

            { Name js ; Value { 
                Term { [ js0[] ] ; In vol{{1}} ; Jacobian jbVolume ; } 
                } 
            }
        
            { Name psMovingForce ; Value {
                Term { [ {qnt_MovingForce} ] ; In domainALL ; Jacobian jbVolume ; }
                }
            }
            { Name forceMoving ; Value {
                Integral { [ - TM[{d qnt_A}] * {d qnt_MovingForce} ] ;
                    In volAir ; Jacobian jbVolume ; Integration igElement ; }
                }
            }
            { Name forceMovingX ; Value {
                Integral { [ CompX[- TM[{d qnt_A}] * {d qnt_MovingForce} ] ] ;
                    In volAir ; Jacobian jbVolume ; Integration igElement ; }
                }
            }
            { Name forceMovingY ; Value {
                Integral { [ CompY[- TM[{d qnt_A}] * {d qnt_MovingForce} ] ] ;
                    In volAir ; Jacobian jbVolume ; Integration igElement ; }
                }
            }
            { Name forceMovingZ ; Value {
                Integral { [ CompZ[- TM[{d qnt_A}] * {d qnt_MovingForce} ] ] ;
                    In volAir ; Jacobian jbVolume ; Integration igElement ; }
                }
            }
        }
    }
}   

";
        #endregion

        #region ---------------------------- 42_PostOperation (*.pro) ----------------------------

        public string m_str42_PostOperation_Script =
        @"#DEFINE,2
# 1 : Coil Name
# 가. 좌하단점
# 2 : X Coord of Left Bottom Point on Roation Plane 
# 3 : Y Coord of Left Bottom Point on Roation Plane 
# 4 : Z Coord of Left Bottom Point on Roation Plane 
# 나. 우하단점
# 5 : X Coord of Right Bottom Point on Roation Plane 
#     Y Coord 는 #3 를 사용함
# 6 : Z Coord of Right Bottom Point on Roation Plane 
# 다. 좌상단점
#     X Coord 는 #2 를 사용함
# 7 : Y Coord of Left Top Point Roation XY Plane 
#     Z Coord 는 #4 를 사용함
# 8 : 자속밀도 벡터 해상도

# Script 생성기에서 주석처리는 첫번째 자리에 # 이 위치할 경우이다. (GetDP 주석은 // 이다.)
# Script 명령어에서 { 기호를 사용하는 경우 {{ 가 발생하지 않도록 주의하라  

PostOperation {
    { Name poMagStatic_A ; NameOfPostProcessing ppMagStatic_A;
        Operation {            

            Echo[ Str[
                ""nView = 1;"",
                ""View[nView].RangeType = 2;"",
                ""View[nView].CustomMax = 1.7;"",
                ""View[nView].CustomMin = 0.0;"",
                ""View[nView].Name = StrCat['Magnetic Density'];"",
                ""Mesh.SurfaceEdges = 0;"" ],
                File ""maps.opt"" ];
            
            Print[ js, OnElementsOf vol{{1}}, File ""js.pos"" ] ;
# 전체 출력은 용량이 너무커서 사용하지 않는다.            
            //Print[ b, OnElementsOf domainALL, File ""b.pos"" ] ;
            
# OnPlane 은 {좌하단점}, {우하단점}, {좌상단점} 순서로 좌표를 입력하고 뒤이어서 {가로 해상도, 세로 해상도} 를 지정한다.
# mm 은 단위 환산이다. ( mm = 1e-3 )
            Print[ b, OnPlane { { {{2}}*mm, {{3}}*mm, {{4}}*mm } { {{5}}*mm, {{3}}*mm, {{6}}*mm } { {{2}}*mm, {{7}}*mm, {{4}}*mm } } { {{8}}, {{8}} }, File ""b_cut.pos"" ];
# Print 수 만큼 View 가 생긴다. 따라서 View 인덱스는 0 부터 시작해서 1이다.

            DeleteFile[""F.dat""];
            DeleteFile [""Fx.dat""];
            DeleteFile [""Fy.dat""];
            DeleteFile [""Fz.dat""];
            
            Print[ forceMoving[volAir], OnGlobal, Format Table, File > ""F.dat""  ];
    
            Print[ forceMovingX[volAir], OnGlobal, Format Table, File > ""Fx.dat"",
              SendToServer Sprintf(""Output/Coil %g/X force [N]"", 1), Color ""Ivory""  ];
              
            Print[ forceMovingY[volAir], OnGlobal, Format Table, File > ""Fy.dat"",
              SendToServer Sprintf(""Output/Coil %g/Y force [N]"", 1), Color ""Ivory""  ];
              
            Print[ forceMovingZ[volAir], OnGlobal, Format Table, File > ""Fz.dat"",
              SendToServer Sprintf(""Output/Coil %g/Z force [N]"", 1), Color ""Ivory""  ];
        }
    }
}

";
        #endregion


        #region ---------------------------- 51_Magnetic Density Vector Image (Image.geo) ----------------------------

        public string m_str51_Image_Script =
        @"#IMPORT,1
# 1 : STEP File Name

# Script 생성기에서 주석처리는 첫번째 자리에 # 이 위치할 경우이다. (GetDP 주석은 // 이다.)
# Script 명령어에서 { 기호를 사용하는 경우 {{ 가 발생하지 않도록 주의하라  

SetFactory(""OpenCASCADE"");

mm = 1e-3;

Merge ""{{1}}"";

STEP_Volumes[] = Volume '*';

Dilate { {0, 0, 0}, {mm, mm, mm} } { Volume{STEP_Volumes[]}; }

General.Trackball = 0;
General.RotationX = 20; General.RotationY = -20; General.RotationZ = 0;

Print ""Image.gif"";

//Exit;

";
        #endregion


    }
}