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

        #region --------------------------- APIs ----------------------------------------
        //-----------------------------------------------------------------------------
        // API 함수 사용
        //-----------------------------------------------------------------------------
        // [주의사항] 꼭 Class 안에 존재해야 함
        //

        [DllImport("user32.dll", EntryPoint = "MoveWindow")]
        public static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        [DllImport("user32.dll")]
        private static extern Boolean ShowWindow(IntPtr hWnd, Int32 nCmdShow);

        [DllImport("user32.dll")]
        public static extern bool GetWindowRect(IntPtr hwnd, ref Rect rectangle);

        public struct Rect
        {
            public int Left { get; set; }
            public int Top { get; set; }
            public int Right { get; set; }
            public int Bottom { get; set; }
        }
        //-----------------------------------------------------------------------------

        #endregion

        private Process m_gmsh = new Process();

        private bool m_bBeforeStart = false;

        public COnelab()
        {

        }

        /// <summary>
        /// 아직 하나의 Gmsh 로 관리가 되고 있지 않아서 모두 지우는 것으로 되어 있다.
        /// 추후 runScript 에서 반복적으로 생성되는 gmsh 를 변경해서 하나의 gmsh 로 관리하라.
        /// </summary>
        public void closeGmsh()
        {
            try
            {
                // 첫번째 실행에서는 m_gmsh.HasExited 오류가 발생해서 제외한다.
                if ( m_bBeforeStart == true)
                    // 연결된 프로세스가 있는지 확인하고 명령을 실행한다.
                    if (false == m_gmsh.HasExited)
                    {
                        m_gmsh.CloseMainWindow();

                        // Free resources associated with process.
                        m_gmsh.Close();
                    }
            }
            catch (Exception ex)
            {
                CNotice.printLog(ex.Message);

                return;
            }
        }

        public void runScript(string strCmd, string strArgs, bool bWaiting = false, ProgressBar progressBar = null)
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
                // 기존에 떠 있는 Gmsh 를 닫는다.
                closeGmsh();

                m_gmsh.StartInfo.FileName = strCmd;
                m_gmsh.StartInfo.Arguments = strArgs;

                m_gmsh.Start();

                m_bBeforeStart = true;

                // 창이 뜨는 시간을 대기한다.
                Thread.Sleep(200);
                resizeGmsh();

                const int TIME_STEP_ms = 500;

                // 프로세스를 기다리게 설정된 경우만 사용된다.
                if (bWaiting == true)
                {
                    // ProgressBar 동작없이 기다리는 프로세스
                    if (progressBar == null)
                    {
                        // 작업을 마칠때 까지 기다린다
                        m_gmsh.WaitForExit();

                        // 중간 중간에 이벤트를 받을 수 있도록 한다
                        System.Windows.Forms.Application.DoEvents();
                    }
                    // ProgressBar 동작을 하며 기다리는 프로세스
                    else
                    {
                        while (!m_gmsh.HasExited)
                        {
                            progressBar.PerformStep();

                            if (progressBar.Value == progressBar.Maximum)
                                progressBar.Value = 0;

                            Thread.Sleep(TIME_STEP_ms);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CNotice.printLog(ex.Message);

                return;
            }
        }

        private void resizeGmsh()
        {
            try
            {
                Rect gmshRect = new Rect();

                const int GMSH_WIDTH = 1200 - 84;
                const int GMSH_HEIGHT = 710 + 67;

                #region ----------------- 모니터 중심에 위치 시킨다. --------------------

                //int iScreenWidth = Screen.PrimaryScreen.Bounds.Width;
                //int iScreenHeight = Screen.PrimaryScreen.Bounds.Height;

                //// Gmsh 가 화면 중심에 위치하도록 좌측과 상측의 마진을 계산한다.
                //if (iScreenWidth > GMSH_WIDTH)
                //    gmshRect.Left = (int)((iScreenWidth - GMSH_WIDTH) / 2.0f);
                //else
                //    gmshRect.Left = 0;

                //if (iScreenHeight > GMSH_HEIGHT)
                //    gmshRect.Top = (int)((iScreenHeight - GMSH_HEIGHT) / 2.0f);
                //else
                //    gmshRect.Top = 0;

                #endregion

                // 연결된 프로세스가 있는지 확인하고 명령을 실행한다.
                if (false == m_gmsh.HasExited)
                {
                    IntPtr ptr = m_gmsh.MainWindowHandle;
                    GetWindowRect(ptr, ref gmshRect);
                }

                // 이미지의 XY 비율이 Gmsh 를 따라가기 때문에 Gmsh 의 크기를 강제로 결정한다.
                // 이번 실행은 이미 위에서 이미지가 만들어져서 어쩔수 없지만
                // 다음번 실행때부터 크기 문제가 없도록 매번 자속밀도 보기 실행때 마다 크기를 재 설정한다.

                // 약간의 지연시간이 있어야 창크기의 조절이 가능하다
                // (100 ms 는 부족하여 안전하게 500 ms 를 사용한다)
                Thread.Sleep(500);
                MoveWindow(m_gmsh.MainWindowHandle, gmshRect.Left, gmshRect.Top, GMSH_WIDTH, GMSH_HEIGHT, true);
            }
            catch (Exception ex)
            {
                CNotice.printLog(ex.Message);

                return;
            }
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

        #region ---------------------------- 01 Show STEP (DesignName.geo or Change.geo in Shape Directory) ----------------------------
        // New Design 과 Change Shape 기능에 사용된다.

        public string m_str01_Show_STEP_Script =
        @"#CHECK_STEP,2
# 1 : Step File Full Name
# 2 : Part Name File Full Name
# 3 : Design Name
# 4 : getdp File Full Name

# Script 생성기에서 주석처리는 첫번째 자리에 # 이 위치할 경우이다. (GetDP 주석은 // 이다.)
# Script 명령어에서 { 기호를 사용하는 경우 {{ 가 발생하지 않도록 주의하라  

SetFactory(""OpenCASCADE"");

Merge ""{{1}}"";

STEP_Volumes[] = Volume '*';

For k In {0 : #STEP_Volumes[]-1}

    stepName = StrReplace(Volume{STEP_Volumes[k]}, ""Shapes/"", """");

    Printf(stepName) >> ""{{2}}"";

EndFor

Geometry.Volumes = 1;
Geometry.VolumeLabels = 1;
Geometry.Color.Volumes = {125,125,125};

# 면처리가 화면은 예쁘지만 내부를 볼 수 없어서 사용하지 않는다.
#Geometry.Points = 0;
#Geometry.Curves = 0;
#Geometry.Surfaces = 1;
#Geometry.SurfaceType = 2;

# 해석목적이 아니라 제품 크기를 계산하기 위해 2D Mesh 진행하고 파일을 보관한다.
Mesh 2;
Mesh.SurfaceEdges = 1;
Mesh.VolumeEdges = 0;

Save ""{{3}}.msh"";

# Gmsh 가 Background 실행으로 바뀌고 부터 
# 첫번째 실행때 getdp 경로 설정이 되지 않은 문제가 있어서 항상 강제로 지정한다.
# 정상적인 경우는 %appdata% 의 gmshrc 파일에 추가 되고 그것을 사용하게 된다.
Solver.Executable0 = ""{{4}}"";

";
        #endregion

        #region ---------------------------- 02 Show Part (Part.geo in Shape or Force Directory) ----------------------------
        // Show Part 와 Full B Vector 기능에 사용된다.

        public string m_str02_Show_Part_Script =
        @"#CHECK_STEP,1
# 1 : Step File Full Name
# 2 : Moving Part Index Number
# 3 : Moving X
# 4 : Moving Y
# 5 : Moving Z
# 6 : getdp File Full Name

# Script 생성기에서 주석처리는 첫번째 자리에 # 이 위치할 경우이다. (GetDP 주석은 // 이다.)
# Script 명령어에서 { 기호를 사용하는 경우 {{ 가 발생하지 않도록 주의하라  

SetFactory(""OpenCASCADE"");

mm = 1e-3;

Merge ""{{1}}"";

STEP_Volumes[] = Volume '*';

Dilate { {0, 0, 0}, {mm, mm, mm} } { Volume{STEP_Volumes[]}; }

volMovingPart = STEP_Volumes[{{2}}];

Translate { {{3}} * mm , {{4}} * mm, {{5}} * mm } { Volume{ volMovingPart }; }

Geometry.Volumes = 1;
Geometry.VolumeLabels = 1;
Geometry.Color.Volumes = {125,125,125};

# 방향을 보기 좋게 틀어 준다
General.Trackball = 0;
General.RotationX = 20; General.RotationY = -20; General.RotationZ = 0;

# 면처리가 화면은 예쁘지만 내부를 볼 수 없어서 사용하지 않는다.
#Geometry.Points = 0;
#Geometry.Curves = 0;
#Geometry.Surfaces = 1;
#Geometry.SurfaceType = 2;

# Gmsh 가 Background 실행으로 바뀌고 부터 
# 첫번째 실행때 getdp 경로 설정이 되지 않은 문제가 있어서 항상 강제로 지정한다.
# 정상적인 경우는 %appdata% 의 gmshrc 파일에 추가 되고 그것을 사용하게 된다.
Solver.Executable0 = ""{{6}}"";

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
# 1 : STEP File Full Name

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
# 12 : Decide to use Steel

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
{{12}}

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
# 2 : First Line String for Steel
# 3 : Second Line String for Steel
# 4 : First Line String for Magnet
# 5 : Second Line String for Magnet

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

{{2}}
{{3}}

{{4}}
{{5}}

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
            
            Print[ js, OnElementsOf vol{{1}}, File ""js.pos"" ] ;
            Print[ b, OnElementsOf domainALL, File ""b.pos"" ] ;
            
# OnPlane 은 {좌하단점}, {우하단점}, {좌상단점} 순서로 좌표를 입력하고 뒤이어서 {가로 해상도, 세로 해상도} 를 지정한다.
# mm 은 단위 환산이다. ( mm = 1e-3 )
            Print[ b, OnPlane { { {{2}}*mm, {{3}}*mm, {{4}}*mm } { {{5}}*mm, {{3}}*mm, {{6}}*mm } { {{2}}*mm, {{7}}*mm, {{4}}*mm } } { {{8}}, {{8}} }, File ""b_cut.pos"" ];

# Print 수 만큼 View 가 생긴다. 따라서 View 인덱스는 0 부터 시작해서 2이다.
            Echo[ Str[
                ""nView = 2;"",
                ""View[nView].RangeType = 2;"",
                ""View[nView].CustomMax = 1.7;"",
                ""View[nView].CustomMin = 0.0;"",
                ""View[nView].SaturateValues = 1;"",
                ""View[nView].Name = StrCat['Magnetic Density'];"",
                ""Mesh.SurfaceEdges = 0;"" ],
                File ""maps.opt"" ];

            DeleteFile [""F.dat""];
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

        #region ---------------------------- 51 Print Image (Image.geo in Force Directory) ----------------------------
        // Secton B Vector 기능에 사용된다.

        public string m_str51_Print_Image_Script =
        @"#CHECK_STEP,1
# 1 : Step File Full Name
# 2 : Moving Part Index Number
# 3 : Moving X
# 4 : Moving Y
# 5 : Moving Z

# Script 생성기에서 주석처리는 첫번째 자리에 # 이 위치할 경우이다. (GetDP 주석은 // 이다.)
# Script 명령어에서 { 기호를 사용하는 경우 {{ 가 발생하지 않도록 주의하라  

SetFactory(""OpenCASCADE"");

mm = 1e-3;

Merge ""{{1}}"";

STEP_Volumes[] = Volume '*';

Dilate { {0, 0, 0}, {mm, mm, mm} } { Volume{STEP_Volumes[]}; }

volMovingPart = STEP_Volumes[{{2}}];

Translate { {{3}} * mm , {{4}} * mm, {{5}} * mm } { Volume{ volMovingPart }; }


# 방향을 보기 좋게 틀어 준다
General.Trackball = 0;
General.RotationX = 20; General.RotationY = -20; General.RotationZ = 0;

Print ""Image.gif"";

Exit;

";
        #endregion
    }
}