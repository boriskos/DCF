@echo off
echo @echo off
echo if not exist Data mkdir Data
for /F %%i in (db_sizes.txt) do (
	for /F %%j in (db_probabilities.txt) do (
		for /F %%l in (db_constraints.txt) do (
			for /L %%k in (0,1,3) do (
				echo echo. ^>^> Data/Output_%%i.txt
				if %%k==0 (
					echo echo ++++++++++ NumberOfUsers = %%i UserProfiles = %%j NumOfTopicsToAnswer = %%l ++++++++++++ ^>^> Data/Output_%%i.txt
					echo del e:\temp\*.dat ^>NUL 2^>NUL
					echo DCF.DemoRules.Test.exe generate_files /UsersCount=%%i /UserProfiles=%%j /NumOfTopicsToAnswer=%%l /WorkingDirectory=e:\temp ^>^> Data/Creation_%%i.txt
					echo mysql -uboris -pboris target ^< e:\temp\!clean_state.sql
					echo mysql -uboris -pboris target ^< e:\temp\!clean_basis_tables.sql
					echo mysql -uboris -pboris target ^< e:\temp\load_all.sql
					echo echo ======== Experiment Cosine =========== ^>^> Data/Output_%%i.txt
					echo DCF.DemoRules.Test.exe read-XML Rules\_CosineRule.xml ^>^> Data/Output_%%i.txt
					echo mysql -uboris -pboris target ^< e:\temp\!count_correct_facts_ratio.sql ^>^> Data/Output_%%i.txt
				) 
				if %%k==1 (
					echo echo ======== Experiment FixedPoint PageRank =========== ^>^> Data/Output_%%i.txt
					echo mysql -uboris -pboris target ^< e:\temp\!clean_state.sql
					echo DCF.DemoRules.Test.exe read-XML Rules\_FixedPageRankRule.xml ^>^> Data/Output_%%i.txt
					echo mysql -uboris -pboris target ^< e:\temp\!count_correct_facts_ratio.sql ^>^> Data/Output_%%i.txt
				)
				if %%k==2 (
					echo echo ======== Experiment Prob PageRank =========== ^>^> Data/Output_%%i.txt
					echo mysql -uboris -pboris target ^< e:\temp\!clean_state.sql
					echo DCF.DemoRules.Test.exe read-XML Rules\_PageRankRule.xml ^>^> Data/Output_%%i.txt
					echo mysql -uboris -pboris target ^< e:\temp\!count_correct_facts_ratio.sql ^>^> Data/Output_%%i.txt
				)
				if %%k==3 (
					echo echo ======== Experiment Majority =========== ^>^> Data/Output_%%i.txt
					echo mysql -uboris -pboris target ^< e:\temp\!clean_state.sql
					echo DCF.DemoRules.Test.exe read-XML Rules\_MajorityRule.xml ^>^> Data/Output_%%i.txt
					echo mysql -uboris -pboris target ^< e:\temp\!count_correct_facts_ratio_maj.sql ^>^> Data/Output_%%i.txt
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
