if not exist Data mkdir Data
echo.
DCF.DemoRules.Test.exe generate /NumberOfFacts=50000 /UserProfiles=(1,0.1) > Data/Creation_50000_0.1.txt
echo ======== Experiment RepairPrimaryKey =========== >> Data/Output_50000_0.1.txt
DCF.DemoRules.Test.exe clean /Experiment=0 >> Data/Output_50000_0.1.txt
echo.
echo ======== Experiment TwoEstimates =========== >> Data/Output_50000_0.1.txt
DCF.DemoRules.Test.exe clean /Experiment=1 >> Data/Output_50000_0.1.txt
echo.
echo ======== Experiment Cosine =========== >> Data/Output_50000_0.1.txt
DCF.DemoRules.Test.exe clean /Experiment=2 >> Data/Output_50000_0.1.txt
echo.
echo ======== Experiment Majority =========== >> Data/Output_50000_0.1.txt
DCF.DemoRules.Test.exe clean /Experiment=3 >> Data/Output_50000_0.1.txt
echo.
DCF.DemoRules.Test.exe generate /NumberOfFacts=50000 /UserProfiles=(1,0.2) > Data/Creation_50000_0.2.txt
echo ======== Experiment RepairPrimaryKey =========== >> Data/Output_50000_0.2.txt
DCF.DemoRules.Test.exe clean /Experiment=0 >> Data/Output_50000_0.2.txt
echo.
echo ======== Experiment TwoEstimates =========== >> Data/Output_50000_0.2.txt
DCF.DemoRules.Test.exe clean /Experiment=1 >> Data/Output_50000_0.2.txt
echo.
echo ======== Experiment Cosine =========== >> Data/Output_50000_0.2.txt
DCF.DemoRules.Test.exe clean /Experiment=2 >> Data/Output_50000_0.2.txt
echo.
echo ======== Experiment Majority =========== >> Data/Output_50000_0.2.txt
DCF.DemoRules.Test.exe clean /Experiment=3 >> Data/Output_50000_0.2.txt
echo.
DCF.DemoRules.Test.exe generate /NumberOfFacts=50000 /UserProfiles=(1,0.3) > Data/Creation_50000_0.3.txt
echo ======== Experiment RepairPrimaryKey =========== >> Data/Output_50000_0.3.txt
DCF.DemoRules.Test.exe clean /Experiment=0 >> Data/Output_50000_0.3.txt
echo.
echo ======== Experiment TwoEstimates =========== >> Data/Output_50000_0.3.txt
DCF.DemoRules.Test.exe clean /Experiment=1 >> Data/Output_50000_0.3.txt
echo.
echo ======== Experiment Cosine =========== >> Data/Output_50000_0.3.txt
DCF.DemoRules.Test.exe clean /Experiment=2 >> Data/Output_50000_0.3.txt
echo.
echo ======== Experiment Majority =========== >> Data/Output_50000_0.3.txt
DCF.DemoRules.Test.exe clean /Experiment=3 >> Data/Output_50000_0.3.txt
echo.
DCF.DemoRules.Test.exe generate /NumberOfFacts=50000 /UserProfiles=(1,0.5) > Data/Creation_50000_0.5.txt
echo ======== Experiment RepairPrimaryKey =========== >> Data/Output_50000_0.5.txt
DCF.DemoRules.Test.exe clean /Experiment=0 >> Data/Output_50000_0.5.txt
echo.
echo ======== Experiment TwoEstimates =========== >> Data/Output_50000_0.5.txt
DCF.DemoRules.Test.exe clean /Experiment=1 >> Data/Output_50000_0.5.txt
echo.
echo ======== Experiment Cosine =========== >> Data/Output_50000_0.5.txt
DCF.DemoRules.Test.exe clean /Experiment=2 >> Data/Output_50000_0.5.txt
echo.
echo ======== Experiment Majority =========== >> Data/Output_50000_0.5.txt
DCF.DemoRules.Test.exe clean /Experiment=3 >> Data/Output_50000_0.5.txt
echo.
DCF.DemoRules.Test.exe generate /NumberOfFacts=50000 /UserProfiles=(1,0.9) > Data/Creation_50000_0.9.txt
echo ======== Experiment RepairPrimaryKey =========== >> Data/Output_50000_0.9.txt
DCF.DemoRules.Test.exe clean /Experiment=0 >> Data/Output_50000_0.9.txt
echo.
echo ======== Experiment TwoEstimates =========== >> Data/Output_50000_0.9.txt
DCF.DemoRules.Test.exe clean /Experiment=1 >> Data/Output_50000_0.9.txt
echo.
echo ======== Experiment Cosine =========== >> Data/Output_50000_0.9.txt
DCF.DemoRules.Test.exe clean /Experiment=2 >> Data/Output_50000_0.9.txt
echo.
echo ======== Experiment Majority =========== >> Data/Output_50000_0.9.txt
DCF.DemoRules.Test.exe clean /Experiment=3 >> Data/Output_50000_0.9.txt
echo.
DCF.DemoRules.Test.exe generate /NumberOfFacts=100000 /UserProfiles=(1,0.1) > Data/Creation_100000_0.1.txt
echo ======== Experiment RepairPrimaryKey =========== >> Data/Output_100000_0.1.txt
DCF.DemoRules.Test.exe clean /Experiment=0 >> Data/Output_100000_0.1.txt
echo.
echo ======== Experiment TwoEstimates =========== >> Data/Output_100000_0.1.txt
DCF.DemoRules.Test.exe clean /Experiment=1 >> Data/Output_100000_0.1.txt
echo.
echo ======== Experiment Cosine =========== >> Data/Output_100000_0.1.txt
DCF.DemoRules.Test.exe clean /Experiment=2 >> Data/Output_100000_0.1.txt
echo.
echo ======== Experiment Majority =========== >> Data/Output_100000_0.1.txt
DCF.DemoRules.Test.exe clean /Experiment=3 >> Data/Output_100000_0.1.txt
echo.
DCF.DemoRules.Test.exe generate /NumberOfFacts=100000 /UserProfiles=(1,0.2) > Data/Creation_100000_0.2.txt
echo ======== Experiment RepairPrimaryKey =========== >> Data/Output_100000_0.2.txt
DCF.DemoRules.Test.exe clean /Experiment=0 >> Data/Output_100000_0.2.txt
echo.
echo ======== Experiment TwoEstimates =========== >> Data/Output_100000_0.2.txt
DCF.DemoRules.Test.exe clean /Experiment=1 >> Data/Output_100000_0.2.txt
echo.
echo ======== Experiment Cosine =========== >> Data/Output_100000_0.2.txt
DCF.DemoRules.Test.exe clean /Experiment=2 >> Data/Output_100000_0.2.txt
echo.
echo ======== Experiment Majority =========== >> Data/Output_100000_0.2.txt
DCF.DemoRules.Test.exe clean /Experiment=3 >> Data/Output_100000_0.2.txt
echo.
DCF.DemoRules.Test.exe generate /NumberOfFacts=100000 /UserProfiles=(1,0.3) > Data/Creation_100000_0.3.txt
echo ======== Experiment RepairPrimaryKey =========== >> Data/Output_100000_0.3.txt
DCF.DemoRules.Test.exe clean /Experiment=0 >> Data/Output_100000_0.3.txt
echo.
echo ======== Experiment TwoEstimates =========== >> Data/Output_100000_0.3.txt
DCF.DemoRules.Test.exe clean /Experiment=1 >> Data/Output_100000_0.3.txt
echo.
echo ======== Experiment Cosine =========== >> Data/Output_100000_0.3.txt
DCF.DemoRules.Test.exe clean /Experiment=2 >> Data/Output_100000_0.3.txt
echo.
echo ======== Experiment Majority =========== >> Data/Output_100000_0.3.txt
DCF.DemoRules.Test.exe clean /Experiment=3 >> Data/Output_100000_0.3.txt
echo.
DCF.DemoRules.Test.exe generate /NumberOfFacts=100000 /UserProfiles=(1,0.5) > Data/Creation_100000_0.5.txt
echo ======== Experiment RepairPrimaryKey =========== >> Data/Output_100000_0.5.txt
DCF.DemoRules.Test.exe clean /Experiment=0 >> Data/Output_100000_0.5.txt
echo.
echo ======== Experiment TwoEstimates =========== >> Data/Output_100000_0.5.txt
DCF.DemoRules.Test.exe clean /Experiment=1 >> Data/Output_100000_0.5.txt
echo.
echo ======== Experiment Cosine =========== >> Data/Output_100000_0.5.txt
DCF.DemoRules.Test.exe clean /Experiment=2 >> Data/Output_100000_0.5.txt
echo.
echo ======== Experiment Majority =========== >> Data/Output_100000_0.5.txt
DCF.DemoRules.Test.exe clean /Experiment=3 >> Data/Output_100000_0.5.txt
echo.
DCF.DemoRules.Test.exe generate /NumberOfFacts=100000 /UserProfiles=(1,0.9) > Data/Creation_100000_0.9.txt
echo ======== Experiment RepairPrimaryKey =========== >> Data/Output_100000_0.9.txt
DCF.DemoRules.Test.exe clean /Experiment=0 >> Data/Output_100000_0.9.txt
echo.
echo ======== Experiment TwoEstimates =========== >> Data/Output_100000_0.9.txt
DCF.DemoRules.Test.exe clean /Experiment=1 >> Data/Output_100000_0.9.txt
echo.
echo ======== Experiment Cosine =========== >> Data/Output_100000_0.9.txt
DCF.DemoRules.Test.exe clean /Experiment=2 >> Data/Output_100000_0.9.txt
echo.
echo ======== Experiment Majority =========== >> Data/Output_100000_0.9.txt
DCF.DemoRules.Test.exe clean /Experiment=3 >> Data/Output_100000_0.9.txt
echo.
DCF.DemoRules.Test.exe generate /NumberOfFacts=500000 /UserProfiles=(1,0.1) > Data/Creation_500000_0.1.txt
echo ======== Experiment RepairPrimaryKey =========== >> Data/Output_500000_0.1.txt
DCF.DemoRules.Test.exe clean /Experiment=0 >> Data/Output_500000_0.1.txt
echo.
echo ======== Experiment TwoEstimates =========== >> Data/Output_500000_0.1.txt
DCF.DemoRules.Test.exe clean /Experiment=1 >> Data/Output_500000_0.1.txt
echo.
echo ======== Experiment Cosine =========== >> Data/Output_500000_0.1.txt
DCF.DemoRules.Test.exe clean /Experiment=2 >> Data/Output_500000_0.1.txt
echo.
echo ======== Experiment Majority =========== >> Data/Output_500000_0.1.txt
DCF.DemoRules.Test.exe clean /Experiment=3 >> Data/Output_500000_0.1.txt
echo.
DCF.DemoRules.Test.exe generate /NumberOfFacts=500000 /UserProfiles=(1,0.2) > Data/Creation_500000_0.2.txt
echo ======== Experiment RepairPrimaryKey =========== >> Data/Output_500000_0.2.txt
DCF.DemoRules.Test.exe clean /Experiment=0 >> Data/Output_500000_0.2.txt
echo.
echo ======== Experiment TwoEstimates =========== >> Data/Output_500000_0.2.txt
DCF.DemoRules.Test.exe clean /Experiment=1 >> Data/Output_500000_0.2.txt
echo.
echo ======== Experiment Cosine =========== >> Data/Output_500000_0.2.txt
DCF.DemoRules.Test.exe clean /Experiment=2 >> Data/Output_500000_0.2.txt
echo.
echo ======== Experiment Majority =========== >> Data/Output_500000_0.2.txt
DCF.DemoRules.Test.exe clean /Experiment=3 >> Data/Output_500000_0.2.txt
echo.
DCF.DemoRules.Test.exe generate /NumberOfFacts=500000 /UserProfiles=(1,0.3) > Data/Creation_500000_0.3.txt
echo ======== Experiment RepairPrimaryKey =========== >> Data/Output_500000_0.3.txt
DCF.DemoRules.Test.exe clean /Experiment=0 >> Data/Output_500000_0.3.txt
echo.
echo ======== Experiment TwoEstimates =========== >> Data/Output_500000_0.3.txt
DCF.DemoRules.Test.exe clean /Experiment=1 >> Data/Output_500000_0.3.txt
echo.
echo ======== Experiment Cosine =========== >> Data/Output_500000_0.3.txt
DCF.DemoRules.Test.exe clean /Experiment=2 >> Data/Output_500000_0.3.txt
echo.
echo ======== Experiment Majority =========== >> Data/Output_500000_0.3.txt
DCF.DemoRules.Test.exe clean /Experiment=3 >> Data/Output_500000_0.3.txt
echo.
DCF.DemoRules.Test.exe generate /NumberOfFacts=500000 /UserProfiles=(1,0.5) > Data/Creation_500000_0.5.txt
echo ======== Experiment RepairPrimaryKey =========== >> Data/Output_500000_0.5.txt
DCF.DemoRules.Test.exe clean /Experiment=0 >> Data/Output_500000_0.5.txt
echo.
echo ======== Experiment TwoEstimates =========== >> Data/Output_500000_0.5.txt
DCF.DemoRules.Test.exe clean /Experiment=1 >> Data/Output_500000_0.5.txt
echo.
echo ======== Experiment Cosine =========== >> Data/Output_500000_0.5.txt
DCF.DemoRules.Test.exe clean /Experiment=2 >> Data/Output_500000_0.5.txt
echo.
echo ======== Experiment Majority =========== >> Data/Output_500000_0.5.txt
DCF.DemoRules.Test.exe clean /Experiment=3 >> Data/Output_500000_0.5.txt
echo.
DCF.DemoRules.Test.exe generate /NumberOfFacts=500000 /UserProfiles=(1,0.9) > Data/Creation_500000_0.9.txt
echo ======== Experiment RepairPrimaryKey =========== >> Data/Output_500000_0.9.txt
DCF.DemoRules.Test.exe clean /Experiment=0 >> Data/Output_500000_0.9.txt
echo.
echo ======== Experiment TwoEstimates =========== >> Data/Output_500000_0.9.txt
DCF.DemoRules.Test.exe clean /Experiment=1 >> Data/Output_500000_0.9.txt
echo.
echo ======== Experiment Cosine =========== >> Data/Output_500000_0.9.txt
DCF.DemoRules.Test.exe clean /Experiment=2 >> Data/Output_500000_0.9.txt
echo.
echo ======== Experiment Majority =========== >> Data/Output_500000_0.9.txt
DCF.DemoRules.Test.exe clean /Experiment=3 >> Data/Output_500000_0.9.txt
echo.
DCF.DemoRules.Test.exe generate /NumberOfFacts=1000000 /UserProfiles=(1,0.1) > Data/Creation_1000000_0.1.txt
echo ======== Experiment RepairPrimaryKey =========== >> Data/Output_1000000_0.1.txt
DCF.DemoRules.Test.exe clean /Experiment=0 >> Data/Output_1000000_0.1.txt
echo.
echo ======== Experiment TwoEstimates =========== >> Data/Output_1000000_0.1.txt
DCF.DemoRules.Test.exe clean /Experiment=1 >> Data/Output_1000000_0.1.txt
echo.
echo ======== Experiment Cosine =========== >> Data/Output_1000000_0.1.txt
DCF.DemoRules.Test.exe clean /Experiment=2 >> Data/Output_1000000_0.1.txt
echo.
echo ======== Experiment Majority =========== >> Data/Output_1000000_0.1.txt
DCF.DemoRules.Test.exe clean /Experiment=3 >> Data/Output_1000000_0.1.txt
echo.
DCF.DemoRules.Test.exe generate /NumberOfFacts=1000000 /UserProfiles=(1,0.2) > Data/Creation_1000000_0.2.txt
echo ======== Experiment RepairPrimaryKey =========== >> Data/Output_1000000_0.2.txt
DCF.DemoRules.Test.exe clean /Experiment=0 >> Data/Output_1000000_0.2.txt
echo.
echo ======== Experiment TwoEstimates =========== >> Data/Output_1000000_0.2.txt
DCF.DemoRules.Test.exe clean /Experiment=1 >> Data/Output_1000000_0.2.txt
echo.
echo ======== Experiment Cosine =========== >> Data/Output_1000000_0.2.txt
DCF.DemoRules.Test.exe clean /Experiment=2 >> Data/Output_1000000_0.2.txt
echo.
echo ======== Experiment Majority =========== >> Data/Output_1000000_0.2.txt
DCF.DemoRules.Test.exe clean /Experiment=3 >> Data/Output_1000000_0.2.txt
echo.
DCF.DemoRules.Test.exe generate /NumberOfFacts=1000000 /UserProfiles=(1,0.3) > Data/Creation_1000000_0.3.txt
echo ======== Experiment RepairPrimaryKey =========== >> Data/Output_1000000_0.3.txt
DCF.DemoRules.Test.exe clean /Experiment=0 >> Data/Output_1000000_0.3.txt
echo.
echo ======== Experiment TwoEstimates =========== >> Data/Output_1000000_0.3.txt
DCF.DemoRules.Test.exe clean /Experiment=1 >> Data/Output_1000000_0.3.txt
echo.
echo ======== Experiment Cosine =========== >> Data/Output_1000000_0.3.txt
DCF.DemoRules.Test.exe clean /Experiment=2 >> Data/Output_1000000_0.3.txt
echo.
echo ======== Experiment Majority =========== >> Data/Output_1000000_0.3.txt
DCF.DemoRules.Test.exe clean /Experiment=3 >> Data/Output_1000000_0.3.txt
echo.
DCF.DemoRules.Test.exe generate /NumberOfFacts=1000000 /UserProfiles=(1,0.5) > Data/Creation_1000000_0.5.txt
echo ======== Experiment RepairPrimaryKey =========== >> Data/Output_1000000_0.5.txt
DCF.DemoRules.Test.exe clean /Experiment=0 >> Data/Output_1000000_0.5.txt
echo.
echo ======== Experiment TwoEstimates =========== >> Data/Output_1000000_0.5.txt
DCF.DemoRules.Test.exe clean /Experiment=1 >> Data/Output_1000000_0.5.txt
echo.
echo ======== Experiment Cosine =========== >> Data/Output_1000000_0.5.txt
DCF.DemoRules.Test.exe clean /Experiment=2 >> Data/Output_1000000_0.5.txt
echo.
echo ======== Experiment Majority =========== >> Data/Output_1000000_0.5.txt
DCF.DemoRules.Test.exe clean /Experiment=3 >> Data/Output_1000000_0.5.txt
echo.
DCF.DemoRules.Test.exe generate /NumberOfFacts=1000000 /UserProfiles=(1,0.9) > Data/Creation_1000000_0.9.txt
echo ======== Experiment RepairPrimaryKey =========== >> Data/Output_1000000_0.9.txt
DCF.DemoRules.Test.exe clean /Experiment=0 >> Data/Output_1000000_0.9.txt
echo.
echo ======== Experiment TwoEstimates =========== >> Data/Output_1000000_0.9.txt
DCF.DemoRules.Test.exe clean /Experiment=1 >> Data/Output_1000000_0.9.txt
echo.
echo ======== Experiment Cosine =========== >> Data/Output_1000000_0.9.txt
DCF.DemoRules.Test.exe clean /Experiment=2 >> Data/Output_1000000_0.9.txt
echo.
echo ======== Experiment Majority =========== >> Data/Output_1000000_0.9.txt
DCF.DemoRules.Test.exe clean /Experiment=3 >> Data/Output_1000000_0.9.txt
echo.
DCF.DemoRules.Test.exe generate /NumberOfFacts=5000000 /UserProfiles=(1,0.1) > Data/Creation_5000000_0.1.txt
echo ======== Experiment RepairPrimaryKey =========== >> Data/Output_5000000_0.1.txt
DCF.DemoRules.Test.exe clean /Experiment=0 >> Data/Output_5000000_0.1.txt
echo.
echo ======== Experiment TwoEstimates =========== >> Data/Output_5000000_0.1.txt
DCF.DemoRules.Test.exe clean /Experiment=1 >> Data/Output_5000000_0.1.txt
echo.
echo ======== Experiment Cosine =========== >> Data/Output_5000000_0.1.txt
DCF.DemoRules.Test.exe clean /Experiment=2 >> Data/Output_5000000_0.1.txt
echo.
echo ======== Experiment Majority =========== >> Data/Output_5000000_0.1.txt
DCF.DemoRules.Test.exe clean /Experiment=3 >> Data/Output_5000000_0.1.txt
echo.
DCF.DemoRules.Test.exe generate /NumberOfFacts=5000000 /UserProfiles=(1,0.2) > Data/Creation_5000000_0.2.txt
echo ======== Experiment RepairPrimaryKey =========== >> Data/Output_5000000_0.2.txt
DCF.DemoRules.Test.exe clean /Experiment=0 >> Data/Output_5000000_0.2.txt
echo.
echo ======== Experiment TwoEstimates =========== >> Data/Output_5000000_0.2.txt
DCF.DemoRules.Test.exe clean /Experiment=1 >> Data/Output_5000000_0.2.txt
echo.
echo ======== Experiment Cosine =========== >> Data/Output_5000000_0.2.txt
DCF.DemoRules.Test.exe clean /Experiment=2 >> Data/Output_5000000_0.2.txt
echo.
echo ======== Experiment Majority =========== >> Data/Output_5000000_0.2.txt
DCF.DemoRules.Test.exe clean /Experiment=3 >> Data/Output_5000000_0.2.txt
echo.
DCF.DemoRules.Test.exe generate /NumberOfFacts=5000000 /UserProfiles=(1,0.3) > Data/Creation_5000000_0.3.txt
echo ======== Experiment RepairPrimaryKey =========== >> Data/Output_5000000_0.3.txt
DCF.DemoRules.Test.exe clean /Experiment=0 >> Data/Output_5000000_0.3.txt
echo.
echo ======== Experiment TwoEstimates =========== >> Data/Output_5000000_0.3.txt
DCF.DemoRules.Test.exe clean /Experiment=1 >> Data/Output_5000000_0.3.txt
echo.
echo ======== Experiment Cosine =========== >> Data/Output_5000000_0.3.txt
DCF.DemoRules.Test.exe clean /Experiment=2 >> Data/Output_5000000_0.3.txt
echo.
echo ======== Experiment Majority =========== >> Data/Output_5000000_0.3.txt
DCF.DemoRules.Test.exe clean /Experiment=3 >> Data/Output_5000000_0.3.txt
echo.
DCF.DemoRules.Test.exe generate /NumberOfFacts=5000000 /UserProfiles=(1,0.5) > Data/Creation_5000000_0.5.txt
echo ======== Experiment RepairPrimaryKey =========== >> Data/Output_5000000_0.5.txt
DCF.DemoRules.Test.exe clean /Experiment=0 >> Data/Output_5000000_0.5.txt
echo.
echo ======== Experiment TwoEstimates =========== >> Data/Output_5000000_0.5.txt
DCF.DemoRules.Test.exe clean /Experiment=1 >> Data/Output_5000000_0.5.txt
echo.
echo ======== Experiment Cosine =========== >> Data/Output_5000000_0.5.txt
DCF.DemoRules.Test.exe clean /Experiment=2 >> Data/Output_5000000_0.5.txt
echo.
echo ======== Experiment Majority =========== >> Data/Output_5000000_0.5.txt
DCF.DemoRules.Test.exe clean /Experiment=3 >> Data/Output_5000000_0.5.txt
echo.
DCF.DemoRules.Test.exe generate /NumberOfFacts=5000000 /UserProfiles=(1,0.9) > Data/Creation_5000000_0.9.txt
echo ======== Experiment RepairPrimaryKey =========== >> Data/Output_5000000_0.9.txt
DCF.DemoRules.Test.exe clean /Experiment=0 >> Data/Output_5000000_0.9.txt
echo.
echo ======== Experiment TwoEstimates =========== >> Data/Output_5000000_0.9.txt
DCF.DemoRules.Test.exe clean /Experiment=1 >> Data/Output_5000000_0.9.txt
echo.
echo ======== Experiment Cosine =========== >> Data/Output_5000000_0.9.txt
DCF.DemoRules.Test.exe clean /Experiment=2 >> Data/Output_5000000_0.9.txt
echo.
echo ======== Experiment Majority =========== >> Data/Output_5000000_0.9.txt
DCF.DemoRules.Test.exe clean /Experiment=3 >> Data/Output_5000000_0.9.txt
echo.
DCF.DemoRules.Test.exe generate /NumberOfFacts=10000000 /UserProfiles=(1,0.1) > Data/Creation_10000000_0.1.txt
echo ======== Experiment RepairPrimaryKey =========== >> Data/Output_10000000_0.1.txt
DCF.DemoRules.Test.exe clean /Experiment=0 >> Data/Output_10000000_0.1.txt
echo.
echo ======== Experiment TwoEstimates =========== >> Data/Output_10000000_0.1.txt
DCF.DemoRules.Test.exe clean /Experiment=1 >> Data/Output_10000000_0.1.txt
echo.
echo ======== Experiment Cosine =========== >> Data/Output_10000000_0.1.txt
DCF.DemoRules.Test.exe clean /Experiment=2 >> Data/Output_10000000_0.1.txt
echo.
echo ======== Experiment Majority =========== >> Data/Output_10000000_0.1.txt
DCF.DemoRules.Test.exe clean /Experiment=3 >> Data/Output_10000000_0.1.txt
echo.
DCF.DemoRules.Test.exe generate /NumberOfFacts=10000000 /UserProfiles=(1,0.2) > Data/Creation_10000000_0.2.txt
echo ======== Experiment RepairPrimaryKey =========== >> Data/Output_10000000_0.2.txt
DCF.DemoRules.Test.exe clean /Experiment=0 >> Data/Output_10000000_0.2.txt
echo.
echo ======== Experiment TwoEstimates =========== >> Data/Output_10000000_0.2.txt
DCF.DemoRules.Test.exe clean /Experiment=1 >> Data/Output_10000000_0.2.txt
echo.
echo ======== Experiment Cosine =========== >> Data/Output_10000000_0.2.txt
DCF.DemoRules.Test.exe clean /Experiment=2 >> Data/Output_10000000_0.2.txt
echo.
echo ======== Experiment Majority =========== >> Data/Output_10000000_0.2.txt
DCF.DemoRules.Test.exe clean /Experiment=3 >> Data/Output_10000000_0.2.txt
echo.
DCF.DemoRules.Test.exe generate /NumberOfFacts=10000000 /UserProfiles=(1,0.3) > Data/Creation_10000000_0.3.txt
echo ======== Experiment RepairPrimaryKey =========== >> Data/Output_10000000_0.3.txt
DCF.DemoRules.Test.exe clean /Experiment=0 >> Data/Output_10000000_0.3.txt
echo.
echo ======== Experiment TwoEstimates =========== >> Data/Output_10000000_0.3.txt
DCF.DemoRules.Test.exe clean /Experiment=1 >> Data/Output_10000000_0.3.txt
echo.
echo ======== Experiment Cosine =========== >> Data/Output_10000000_0.3.txt
DCF.DemoRules.Test.exe clean /Experiment=2 >> Data/Output_10000000_0.3.txt
echo.
echo ======== Experiment Majority =========== >> Data/Output_10000000_0.3.txt
DCF.DemoRules.Test.exe clean /Experiment=3 >> Data/Output_10000000_0.3.txt
echo.
DCF.DemoRules.Test.exe generate /NumberOfFacts=10000000 /UserProfiles=(1,0.5) > Data/Creation_10000000_0.5.txt
echo ======== Experiment RepairPrimaryKey =========== >> Data/Output_10000000_0.5.txt
DCF.DemoRules.Test.exe clean /Experiment=0 >> Data/Output_10000000_0.5.txt
echo.
echo ======== Experiment TwoEstimates =========== >> Data/Output_10000000_0.5.txt
DCF.DemoRules.Test.exe clean /Experiment=1 >> Data/Output_10000000_0.5.txt
echo.
echo ======== Experiment Cosine =========== >> Data/Output_10000000_0.5.txt
DCF.DemoRules.Test.exe clean /Experiment=2 >> Data/Output_10000000_0.5.txt
echo.
echo ======== Experiment Majority =========== >> Data/Output_10000000_0.5.txt
DCF.DemoRules.Test.exe clean /Experiment=3 >> Data/Output_10000000_0.5.txt
echo.
DCF.DemoRules.Test.exe generate /NumberOfFacts=10000000 /UserProfiles=(1,0.9) > Data/Creation_10000000_0.9.txt
echo ======== Experiment RepairPrimaryKey =========== >> Data/Output_10000000_0.9.txt
DCF.DemoRules.Test.exe clean /Experiment=0 >> Data/Output_10000000_0.9.txt
echo.
echo ======== Experiment TwoEstimates =========== >> Data/Output_10000000_0.9.txt
DCF.DemoRules.Test.exe clean /Experiment=1 >> Data/Output_10000000_0.9.txt
echo.
echo ======== Experiment Cosine =========== >> Data/Output_10000000_0.9.txt
DCF.DemoRules.Test.exe clean /Experiment=2 >> Data/Output_10000000_0.9.txt
echo.
echo ======== Experiment Majority =========== >> Data/Output_10000000_0.9.txt
DCF.DemoRules.Test.exe clean /Experiment=3 >> Data/Output_10000000_0.9.txt
