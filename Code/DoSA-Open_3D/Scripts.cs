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
using Experiments;
using Nodes;

using gtLibrary;
//using DoSA;
using System.Windows.Forms;
using System.Resources;

namespace Onelab
{
    public enum EMExperimentType
    {
        FORCE_EXPERIMENT,
        STROKE_EXPERIMENT,
        CURRENT_EXPERIMENT,
        MOVEMENT_EXPERIMENT
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
                CNotice.printTrace(ex.Message);
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
                CNotice.printTrace(ex.Message);
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
            //string strRunScriptFileFullNameMaxwell = m_manageFile.solveSpacePC(m_strRunScriptFileFullName);

            //string strArguments = " " + strRunScriptFileFullNameMaxwell;

            //runScript(strMaxwellFileFullName, strArguments, m_strRunScriptFileFullName, true, progressBarMovement);

            //progressBarMovement.Value = progressBarTime.Maximum;
            //------------------------------------------------------------

            try
            {
                m_process = new System.Diagnostics.Process();
                m_process.StartInfo.FileName = strCmd;
                m_process.StartInfo.Arguments = strArgs;

                // 자속밀도의 그림이 범례의 문제가 없으려면 Maxwell 이 최대화가 되어야 한다. 
                // 
                //m_process.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Maximized;
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
                CNotice.printTrace(ex.Message);
            }
        }

        public static void moveGmsh(int iPosX, int iPosY, int iSizeX = 1024, int iSizeY = 768)
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


    public class CScriptContents
    {
        //--------------------------------------------------
        // 주의사항
        //--------------------------------------------------
        // - 내부에 사용되는 { 기호와 {{}} 는 분리하여 꼭 사용하라.
        //

        #region =========================== 01_Read Part Name ===========================

        public string m_str01_CheckSTEP_Script =
        @"#CHECK_STEP,2
# 1 : Step File Full Name
# 2 : Part Name File Fule Name

SetFactory(""OpenCASCADE"");

Merge ""{{1}}"";

STEP_Volumes[] = Volume '*';

For k In {0 : #STEP_Volumes[]-1}

	stepName = StrReplace(Volume{STEP_Volumes[k]}, ""Shapes/"", """");
	
	Printf(stepName) >> ""{{2}}"";
	
EndFor

";
        #endregion

        #region =========================== 02_Define.geo ===========================

        ///----------------------------------------------
        /// Define 번호 규칙
        ///----------------------------------------------
        /// 
        /// Part Objects    : 1 ~ 99
        ///  - 1 : Air
        ///  - 2 : Parts ~
        /// Part Skins      : 101 ~ 199
        ///  - 101 : Air Skin
        ///  - 102 : Parts Skin ~
        ///  
        public string m_str02_Define_Script =
        @"#DEFINE,0

mm = 1e-3;

AIR = 1;
SKIN_AIR = 101;

SKIN_MOVING = 201;
SKIN_STEEL = 202;

";
        #endregion

        public bool getScriptBH(string strMaterialName, ref string strScriptBH, List<double> listH, List<double> listB)
        {
            string strB, strH;

            strB = "    Mat_" + strMaterialName + "_B() = {\n    ";

            foreach(double dB in listB)
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

        #region =========================== 03_BH.pro ===========================

        public string m_str03_BH_Calulate_Script =
        @"#BH,1
# 1 : Material Name

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

        #region =========================== 04_Import STEP (*.geo) ===========================

        public string m_str04_Import_Script =
        @"#IMPORT,1
# 1 : STEP File Name
SetFactory(""OpenCASCADE"");

Include ""Define.geo"";

Mesh.Optimize = 1;
Mesh.VolumeEdges = 0;
Solver.AutoMesh = 2;

Merge ""{{1}}"";

STEP_Volumes[] = Volume '*';

# {{ 가 발생하지 않도록 주의하라 
Dilate { {0, 0, 0}, {mm, mm, mm} } { Volume{STEP_Volumes[]}; }
";
        #endregion

        #region =========================== 04_1_Air_Region (*.geo) ===========================

        public string m_str04_1_Region_Script =
        @"#IMPORT,8
# 1 : Air Mesh Size
# 2 : Part Mesh Size
# 3 : Shape Min X
# 4 : Shape Min Y
# 5 : Shape Min Z
# 6 : Shape Width X
# 7 : Shape Width Y
# 8 : Shape Width Z

Mesh.CharacteristicLengthFactor = {{1}};
Characteristic Length { PointsOf{ Volume{STEP_Volumes[]}; } } = {{2}};

volBox = newv; Box(newv) = { {{3}}*mm, {{4}}*mm, {{5}}*mm, {{6}}*mm, {{7}}*mm, {{8}}*mm };

volAir = newv; BooleanDifference(newv) = { Volume{volBox}; Delete; }{ Volume{STEP_Volumes()}; };

BooleanFragments{ Volume{volAir(), STEP_Volumes()}; Delete; }{}

Physical Surface(SKIN_MOVING) = skinMoving();
Physical Surface(SKIN_STEEL) = skinSteel();

volAll() = Volume '*';
skinAir() = CombinedBoundary{ Volume{ volAll() }; };

Physical Volume(AIR) = volAir;
Physical Surface(SKIN_AIR) = skinAir();

";
        #endregion

        #region =========================== 05_Magnetic Density Vector Image (Image.geo) ===========================

        public string m_str05_Image_Script =
        @"#IMPORT,1
# 1 : STEP File Name
# 2 : Part Mesh Size
SetFactory(""OpenCASCADE"");

mm = 1e-3;

Mesh.Optimize = 1;
Mesh.VolumeEdges = 0;
Solver.AutoMesh = 2;

Merge ""{{1}}"";

STEP_Volumes[] = Volume '*';

Dilate { {0, 0, 0}, {mm, mm, mm} } { Volume{STEP_Volumes[]}; }

Characteristic Length { PointsOf{ Volume{STEP_Volumes[]}; } } = {{2}};

General.Trackball = 0;
General.RotationX = 10; General.RotationY = 0; General.RotationZ = 0;

Mesh 2;

Print ""Image.gif"";

Exit;

";
        #endregion

        #region =========================== 06_Group (*.pro) ===========================

        public string m_str06_Group_Script =
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

        #region =========================== 07_Function (*.pro) ===========================

        public string m_str07_Function_Script =
        @"#DEFINE,0

Function {	

    mu0 = 4*Pi*1e-7;

    Nb_max_iter = 30;
    stop_criterion = 1e-3;
    relaxation_factor = 1;		

    mu[volAir] = mu0;
    nu[volAir] = 1.0/mu0;

";
        #endregion

        #region =========================== 08_09_10_Constraint (*.pro) ===========================

        public string m_str08_Constraint_Script =
        @"#DEFINE,0

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
			{ Region Region[{domainALL}] ; SubRegion domainSkin_A_NoGauge ; Value 0. ; }
		}
	}	
}

FunctionSpace {
	
	{ Name fsHcurl_A_3D ; Type Form1 ;
		BasisFunction {
			{ 	Name se ; NameOfCoef ae ; Function BF_Edge ;
				Support domainHcurl_A ; Entity EdgesOf[ All ] ; }
		}
		Constraint {
			{ 	NameOfCoef ae;	EntityType EdgesOf ; NameOfConstraint cstDirichlet_A_0 ; }
			
			{	NameOfCoef ae  ;	EntityType EdgesOfTreeIn ; EntitySubType StartingOn ;
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
			{	Type Gauss ;
				Case {
					{ GeoElement Triangle    ; NumberOfPoints  4 ; }
					{ GeoElement Quadrangle  ; NumberOfPoints  4 ; }
					{ GeoElement Tetrahedron ; NumberOfPoints  4 ; }
					{ GeoElement Hexahedron  ; NumberOfPoints  6 ; }
					{ GeoElement Prism       ; NumberOfPoints  21 ; }
					{ GeoElement Line        ; NumberOfPoints  4 ; }
				}
			}
		}
	}
}

";
        #endregion

        #region =========================== 11_Formulation_Resolution (*.pro) ===========================

        public string m_str11_Formulation_Resolution_Script =
        @"#DEFINE,3
# 1 : Coil Name
# 2 : Magnet Part String - 1
# 3 : Magnet Part String - 2

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

        #region =========================== 12_PostProcessing (*.pro) ===========================

        public string m_str12_PostProcessing_Script =
        @"#DEFINE,0

PostProcessing {

	{ Name ppMagStatic_A ; NameOfFormulation fmMagStatic_A ;
		PostQuantity {			
			
			{ Name b ; Value { 
				Term { [ {d qnt_A} ]; In domainHcurl_A; Jacobian jbVolume; } 
				} 
			}	

            { Name js ; Value { 
				Term { [ js0[] ] ; In volCoil ; Jacobian jbVolume ; } 
				} 
			}	
		
			{ Name psMovingForce ; Value {
				Term { [ {qnt_MovingForce} ] ; In domainHcurl_A ; Jacobian jbVolume ; }
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

        #region =========================== 13_PostOperation (*.pro) ===========================

        public string m_str13_PostOperation_Script =
        @"#DEFINE,2
# 1 : Coil Name
# 2 : X Coord of Left Bottom Point on XY Plane 
# 3 : Y Coord of Left Bottom Point on XY Plane 
# 4 : X Coord of Right Bottom Point on XY Plane 
# 5 : Y Coord of Left Top Point on XY Plane 

PostOperation {
	{ Name poMagStatic_A ; NameOfPostProcessing ppMagStatic_A;
		Operation {			
			
			Print[ b, OnElementsOf domainALL, File ""b.pos"" ] ;			
			Print[ b, OnPlane{ { {{2}}*mm, {{3}}*mm,0} { {{4}}*mm, {{3}}*mm,0} { {{2}}*mm, {{5}}*mm,0} } {100, 100}, File ""b_cut.pos"" ];
			
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

#OnelabRun

";
        #endregion
    }
}