@echo off
echo @echo off
echo if not exist Data mkdir Data
for /F %%i in (db_sizes.txt) do (
	for /F %%j in (db_probabilities.txt) do (
		for /F %%l in (db_constraints.txt) do (
			for /L %%k in (0,1,3) do (
				echo echo. ^>^> Data/Output_%%i.txt
				if %%k==0 (
					echo echo ++++++++++ NumberOfFacts = %%i UserProfiles = %%j TopicsVariabilityProfile = %%l ++++++++++++ ^>^> Data/Output_%%i.txt
					echo DCF.DemoRules.Test.exe generate /NumberOfFacts=%%i /UserProfiles=%%j /TopicsVariabilityProfile=%%l ^>^> Data/Creation_%%i.txt
					echo echo ======== Experiment RepairPrimaryKey =========== ^>^> Data/Output_%%i.txt
				) 
				if %%k==1 (
					echo echo ======== Experiment TwoEstimates =========== ^>^> Data/Output_%%i.txt
				)
				if %%k==2 (
					echo echo ======== Experiment Cosine =========== ^>^> Data/Output_%%i.txt
				)
				if %%k==3 (
					echo echo ======== Experiment Majority =========== ^>^> Data/Output_%%i.txt
				)
				for /L %%t in (1,1,3) do (
					echo echo -------- try %%t ------------- ^>^> Data/Output_%%i.txt
					echo DCF.DemoRules.Test.exe clean /ExperimentType=%%k ^>^> Data/Output_%%i.txt
				)
			)
		)
	)
)

echo on
@exit /b 0
:usage
echo usage: create_run_bat.bat ^<SQL query result file^>
echo on
