@echo off
echo if not exist Data mkdir Data
for /F %%i in (db_sizes.txt) do (
	for /F %%j in (db_probabilities.txt) do (
		for /L %%k in (0,1,3) do (
			echo echo.
			if %%k==0 (
				echo DCF.DemoRules.Test.exe generate /NumberOfFacts=%%i /UserProfiles=(1,%%j^) ^> Data/Creation_%%i_%%j.txt
				echo echo ======== Experiment RepairPrimaryKey =========== ^>^> Data/Output_%%i_%%j.txt
			) 
			if %%k==1 (
				echo echo ======== Experiment TwoEstimates =========== ^>^> Data/Output_%%i_%%j.txt
			)
			if %%k==2 (
				echo echo ======== Experiment Cosine =========== ^>^> Data/Output_%%i_%%j.txt
			)
			if %%k==3 (
				echo echo ======== Experiment Majority =========== ^>^> Data/Output_%%i_%%j.txt
			)
			for /L %%t in (1,1,3) do (
				echo echo -------- try %%t ------------- ^>^> Data/Output_%%i_%%j.txt
				echo DCF.DemoRules.Test.exe clean /ExperimentType=%%k ^>^> Data/Output_%%i_%%j.txt
			)
		)
	)
)

echo on
@exit /b 0
:usage
echo usage: create_run_bat.bat ^<SQL query result file^>
echo on
