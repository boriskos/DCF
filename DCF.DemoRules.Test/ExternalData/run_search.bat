if not exist Data mkdir Data
echo.
DCF.DemoRules.Test.exe generate /NumberOfFacts=10000 /UserProfiles=(0.8,0.2)(0.2,0.8) > Data/Creation_1.txt
mysql  --user=boris --password=boris triviamasster < "ExternalData\CheckSupport.sql" > Data\Sample_1.txt
echo ======== Experiment RepairPrimaryKey =========== >> Data/Output_1.txt
DCF.DemoRules.Test.exe clean /Experiment=0 >> Data/Output_1.txt
echo.
echo ======== Experiment TwoEstimates =========== >> Data/Output_1.txt
DCF.DemoRules.Test.exe clean /Experiment=1 >> Data/Output_1.txt
echo.
echo ======== Experiment Cosine =========== >> Data/Output_1.txt
DCF.DemoRules.Test.exe clean /Experiment=2 >> Data/Output_1.txt
echo.
echo ======== Experiment Majority =========== >> Data/Output_1.txt
DCF.DemoRules.Test.exe clean /Experiment=3 >> Data/Output_1.txt


echo.
DCF.DemoRules.Test.exe generate /NumberOfFacts=10000 /UserProfiles=(0.8,0.1)(0.2,1.0) > Data/Creation_2.txt
mysql  --user=boris --password=boris triviamasster < "ExternalData\CheckSupport.sql" > Data\Sample_2.txt
echo ======== Experiment RepairPrimaryKey =========== >> Data/Output_2.txt
DCF.DemoRules.Test.exe clean /Experiment=0 >> Data/Output_2.txt
echo.
echo ======== Experiment TwoEstimates =========== >> Data/Output_2.txt
DCF.DemoRules.Test.exe clean /Experiment=1 >> Data/Output_2.txt
echo.
echo ======== Experiment Cosine =========== >> Data/Output_2.txt
DCF.DemoRules.Test.exe clean /Experiment=2 >> Data/Output_2.txt
echo.
echo ======== Experiment Majority =========== >> Data/Output_2.txt
DCF.DemoRules.Test.exe clean /Experiment=3 >> Data/Output_2.txt


echo.
DCF.DemoRules.Test.exe generate /NumberOfFacts=10000 /UserProfiles=(0.7,0.2)(0.2,0.6)(0.1,0.9) > Data/Creation_3.txt
mysql  --user=boris --password=boris triviamasster < "ExternalData\CheckSupport.sql" > Data\Sample_3.txt
echo ======== Experiment RepairPrimaryKey =========== >> Data/Output_3.txt
DCF.DemoRules.Test.exe clean /Experiment=0 >> Data/Output_3.txt
echo.
echo ======== Experiment TwoEstimates =========== >> Data/Output_3.txt
DCF.DemoRules.Test.exe clean /Experiment=1 >> Data/Output_3.txt
echo.
echo ======== Experiment Cosine =========== >> Data/Output_3.txt
DCF.DemoRules.Test.exe clean /Experiment=2 >> Data/Output_3.txt
echo.
echo ======== Experiment Majority =========== >> Data/Output_3.txt
DCF.DemoRules.Test.exe clean /Experiment=3 >> Data/Output_3.txt


echo.
DCF.DemoRules.Test.exe generate /NumberOfFacts=10000 /UserProfiles=(0.6,0.4)(0.4,0.6) > Data/Creation_4.txt
mysql  --user=boris --password=boris triviamasster < "ExternalData\CheckSupport.sql" > Data\Sample_4.txt
echo ======== Experiment RepairPrimaryKey =========== >> Data/Output_4.txt
DCF.DemoRules.Test.exe clean /Experiment=0 >> Data/Output_4.txt
echo.
echo ======== Experiment TwoEstimates =========== >> Data/Output_4.txt
DCF.DemoRules.Test.exe clean /Experiment=1 >> Data/Output_4.txt
echo.
echo ======== Experiment Cosine =========== >> Data/Output_4.txt
DCF.DemoRules.Test.exe clean /Experiment=2 >> Data/Output_4.txt
echo.
echo ======== Experiment Majority =========== >> Data/Output_4.txt
DCF.DemoRules.Test.exe clean /Experiment=3 >> Data/Output_4.txt


echo.
DCF.DemoRules.Test.exe generate /NumberOfFacts=10000 /UserProfiles=(0.5,0.05)(0.5,0.8) > Data/Creation/ExperimentType=.txt
mysql  --user=boris --password=boris triviamasster < "ExternalData\CheckSupport.sql" > Data\Sample/ExperimentType=.txt
echo ======== Experiment RepairPrimaryKey =========== >> Data/Output/ExperimentType=.txt
DCF.DemoRules.Test.exe clean /ExperimentType=0 >> Data/Output/ExperimentType=.txt
echo.
echo ======== Experiment TwoEstimates =========== >> Data/Output/ExperimentType=.txt
DCF.DemoRules.Test.exe clean /ExperimentType=1 >> Data/Output/ExperimentType=.txt
echo.
echo ======== Experiment Cosine =========== >> Data/Output/ExperimentType=.txt
DCF.DemoRules.Test.exe clean /ExperimentType=2 >> Data/Output/ExperimentType=.txt
echo.
echo ======== Experiment Majority =========== >> Data/Output/ExperimentType=.txt
DCF.DemoRules.Test.exe clean /ExperimentType=3 >> Data/Output/ExperimentType=.txt



echo.
DCF.DemoRules.Test.exe generate /NumberOfFacts=10000 /UserProfiles=(0.7,0.2)(0.3,0.7) > Data/Creation_6.txt
mysql  --user=boris --password=boris triviamasster < "ExternalData\CheckSupport.sql" > Data\Sample_6.txt
echo ======== Experiment RepairPrimaryKey =========== >> Data/Output_6.txt
DCF.DemoRules.Test.exe clean /ExperimentType=0 >> Data/Output_6.txt
echo.
echo ======== Experiment TwoEstimates =========== >> Data/Output_6.txt
DCF.DemoRules.Test.exe clean /ExperimentType=1 >> Data/Output_6.txt
echo.
echo ======== Experiment Cosine =========== >> Data/Output_6.txt
DCF.DemoRules.Test.exe clean /ExperimentType=2 >> Data/Output_6.txt
echo.
echo ======== Experiment Majority =========== >> Data/Output_6.txt
DCF.DemoRules.Test.exe clean /ExperimentType=3 >> Data/Output_6.txt

