<?php
//---------------------------------------------	
/* API - functions to call
- getLeaderBoard
	- Takes no arguments
	- Returns a list of top ten users in the form of a string: "&username=points"
	
- getUserStatistics
	- Takes no arguments
	- Returns string indicating number of users logged in in past 7 days
	
- getUserInformation
	- Takes userID 
	- Returns userRank, self earned points and points earned from dividends. Adds a new user to the database if the user doesn't exist
	
- getNewCategory
	- Takes the ID of previous category seen by user (so that the random generator doesn't repeat categories)
	- Returns a name and ID of a new random category
	
- checkNameSubmission
	- Takes userID, ID of current category seem by user and item submitted to be checked
	- Returns 
- getUpdatedItemList
	- Takes UserID, current category
	- Returns a String containing all item mentions in the category and the number of mentions for each item

*/
//---------------------------------------------	

include('DBsettings.php');

// All variables are always returned as an array
$returnVars = array();
$returnVars['index'] = "1";

//The file receives a function request from the front-end section. This is because there is no serialization between actionscript and PHP
$functionToRun = $_POST['functions']; 


//Connect to the database
mysql_connect($address, $username, $password) or die(mysql_error());
mysql_select_db($databaseName) or die(mysql_error());

/*
Selection of a function to run based on instructions from actionscript
*/
switch ($functionToRun){
//---------------------------------------------		
	case 'getLeaderBoard':
		$result = mysql_query("SELECT * FROM `users` ORDER BY Points+dividendPoints DESC LIMIT 15") or die(mysql_error());
		$itemList = "";
		$position = 1;
		
		while($row = mysql_fetch_array($result)){
			$sum = $row['dividendPoints'] + $row['Points'];
			
			$userEmail = $row['userID'];
			
			if ($row['userName'] == ""){
				$xml = simplexml_load_file('http://bluepages.ibm.com/BpHttpApis/slaphapi?ibmperson/(mail='.$userEmail.').list/byxml?hrLastName&hrFirstName&uid');
				
				$lastName = (string)$xml->{'directory-entries'}[0]->entry[0]->attr[0]->value[0];
				$firstName = (string)$xml->{'directory-entries'}[0]->entry[0]->attr[2]->value[0];
				
				$lastName = ucfirst(strtolower($lastName));
				$firstName = ucfirst(strtolower($firstName));
				$username = $firstName." ".$lastName;
				
				$username = mysql_real_escape_string($username);
				
				mysql_query("UPDATE `users` SET userName = '$username' WHERE userID = '$userEmail'");
			} else {
				$username = $row['userName'];
			}
			if ($username != ""){
			$itemList = $itemList."&".$username."=".$sum."=".$position;
			} else {
				$itemList = $itemList."&".$row['userID']."=".$sum."=".$position;
			}
			
			$position += 1;
		}	
		
		$returnVars['rankedList'] = $itemList;
		$returnVars['functionSent'] = "getLeaderBoard";
		break;
//---------------------------------------------				
	case 'getUserStatistics':
		$returnVars['statistics'] = playersLast7Days();
		break;
//---------------------------------------------		

	/*************************
	* Tal, 2010-02-07
	* Creating new user at DB TABLE users.
	*************************/
	case 'registerUser':
	
		// read vars [from Flash]
		$registrationFullname = $_POST['registrationFullname'];
		$registrationFullname  = mysql_real_escape_string($registrationFullname);
		
		$registrationEmail = $_POST['registrationEmail'];
		$registrationEmail  = mysql_real_escape_string($registrationEmail);
		
		$registrationPassword = $_POST['registrationPassword'];
		$registrationPassword  = mysql_real_escape_string($registrationPassword);
		
		/*** Ohad, 2010-02-10, SENDING AN EMAIL TO THE REGISTERED USERS ***/
		$subject = "Welcome to GUESS - Registration Details (please save and do not reply)";
		$message = "Hi " . $registrationFullname . " ! \n\n Thank you for registering to GUESS. \n\n These are your details: \n - Email: " . 
						$registrationEmail . "\n - Password: " . $registrationPassword . "\n" .
						"\n\n Please join and start playing till you reach the top of the leader board ! \n\n" . " (-) GUESS team";
		$from = "guess@il.ibm.com";
		$headers = "";
		mail($registrationEmail,$subject,$message,$headers);
		
		// Creating new user at DB TABLE users:
		mysql_query(
			"INSERT INTO users
			(`userID`, `password`,`userName`,`time`)
			VALUES 
			('"
			. $registrationEmail ."', '" 
			. $registrationPassword . "', '" 
			. $registrationFullname . "', NOW());"
			);
		
		
		// return vars to Flash:
		$returnVars['status'] = 1;
		
		break;
		
		
			
			
	
	/*************************
	* Tal, 2010-02-04:
	* Validating Login details from Flash. returns success code (1/0), userName and userId. 
	**************************/
	case 'authenticateUser':
		
		//getUserStatistics($_POST['username'],$_POST['pass']);
		//*
		$myuser = $_POST['username'];
		$mypass = $_POST['pass'];
		
		//$myuser = $myuser."@il.ibm.com";
		
		$result = mysql_query(
			"SELECT userName, userID 
			FROM users 
			WHERE (userID='$myuser') AND (password='$mypass')");
		
//		$result = mysql_query("UPDATE `users` SET time = now() WHERE userID = '$myuser'");  
		$row = mysql_fetch_array($result);
		
		if ($row != false){
		//$returnVars['userpass'] = $myuser." and ".$mypass.$_SESSION['user'];
			$returnVars['verified'] = 1; //authenticate();
			$returnVars['userName'] = $row['userName']; 
			$returnVars['userID'] = $row['userID'];
		} else {
			$returnVars['verified'] = 0; 
		}
		
//				$returnVars['verified'] = authenticate();
		//*/
		break;
//---------------------------------------------			
	case 'getUserInformation':
		$user = $_POST['userID'];
		$user = mysql_real_escape_string($user);
		
		$result = mysql_query("UPDATE `users` SET time = now() WHERE userID = '$user'");  
		$result = mysql_query("SELECT Points,dividendPoints,pointsSinceLastLogin FROM `users` WHERE userID = '$user' LIMIT 1") or die(mysql_error());  
		$row = mysql_fetch_array($result);
		
		if ($row == false){
			mysql_query("INSERT INTO `users` (userID,Points) VALUES ('$user','0')");
			$returnVars['userPoints'] = 0;
			$returnVars['dividendPoints'] = 0;
			$returnVars['pointsSinceLastLogin'] = 0;
		} else {
			$returnVars['userPoints'] = $row['Points'];
			$returnVars['dividendPoints'] = $row['dividendPoints'];
			$returnVars['pointsSinceLastLogin'] = $row['pointsSinceLastLogin'];
		}
		
		$result = mysql_query("SELECT COUNT(*) FROM users WHERE points + dividendPoints >= (SELECT points+dividendPoints FROM users WHERE userID = '$user')") or die(mysql_error());  
		$row = mysql_fetch_array($result);
		$returnVars['userRank'] = $row['COUNT(*)'];	
		$returnVars['newCategory'] = "false";
		
		$returnVars['statistics'] = playersOnline();

		mysql_query("UPDATE `users` SET pointsSinceLastLogin = 0 WHERE userID = '$user'");
		$returnVars['functionSent'] = "getUserInformation";
		
		break;
//---------------------------------------------				
	//Randomly selects a future category from the database.
	case 'getNewCategory':
		$myuser = $_POST['userID'];
		$previousCategoryID = $_POST['previousCategory'];
		$clickedOnNext = $_POST['clickedOnNext'];
		
		$myuser = mysql_real_escape_string($myuser);
		$previousCategoryID = mysql_real_escape_string($previousCategoryID);
		$clickedOnNext = mysql_real_escape_string($clickedOnNext);
		
		mysql_query("UPDATE `users` SET time = now() WHERE userID = '$myuser'");  
		
		if ($clickedOnNext == "true"){
			$result = mysql_query("SELECT Points FROM `users` WHERE `Points` = 0 AND userID = '$myuser'");
			$row = mysql_fetch_array($result);
			if ($row == false){
				mysql_query("UPDATE `users` SET Points = Points-1 WHERE userID = '$myuser'");
			}
		}
		
		$result = mysql_query("SELECT value FROM `globalvariables` WHERE `key` = 'currentCategoryID'") or die(mysql_error());  
		$row = mysql_fetch_array($result);
		
		$rnd_num = rand(1,100);
		if ($rnd_num <= 50) { // query the single answer question
			if ($row['value'] == $previousCategoryID){
				//$result2 = mysql_query("SELECT * FROM `topics` WHERE TopicId <> '$previousCategoryID' ORDER BY RAND() LIMIT 1") or die(mysql_error());
				$result2 = mysql_query("select tpc.* from topics tpc, (select sf.topicid, max(sf.score) Score from scoredfacts sf where sf.topicid in (select tt.topicid topicid from topics tt where tt.topictype=0 and tt.TopicId <> '$previousCategoryID' and tt.Category='Demo' and tt.topicid not in (select distinct t.topicid from topics t, itemsmentions im where im.userid='$myuser' and t.topicid=im.topicid)) group by sf.topicid order by Score LIMIT 1) a where a.topicid=tpc.topicid") or die(mysql_error());
			} else {
				$newEntry = $row['value'];
				$result2 = mysql_query("SELECT * FROM `topics` WHERE TopicId = '$newEntry'") or die(mysql_error());
			}
		} else { // query multiple answers
			$result2 = mysql_query("SELECT * FROM `topics` WHERE TopicId <> '$previousCategoryID' and TopicType=1 ORDER BY RAND() LIMIT 1") or die(mysql_error());
		}
	
		$row2 = mysql_fetch_array($result2);
		if ($row2 == false)
		{
			$result2 = mysql_query("SELECT * FROM `topics` WHERE TopicId <> '$previousCategoryID' and TopicType=1 ORDER BY RAND() LIMIT 1") or die(mysql_error());
			$row2 = mysql_fetch_array($result2);
		}
		$returnVars['category'] = $row2['TopicName'];
		$returnVars['categoryID'] = $row2['TopicId'];
		$returnVars['newCategory'] = "true";
		$value = $row2['TopicId'];
		
		mysql_query("UPDATE `globalvariables` SET `value` = '$value' WHERE `key` = 'currentCategoryID'");
		$returnVars['functionSent'] = "getNewCategory";
		
		break;
//---------------------------------------------		
	case 'settings':
		//This function does not currently exist in the front-end.
		break;
//---------------------------------------------		
	case 'checkNameSubmission':
		/* ---------------------Checks--------------------- */
		$submitCategory = $_POST['submitCategory'];
		$submitName = $_POST['submitName'];
		$userID = $_POST['userID'];
		$reSubmit = $_POST['reSubmit'];
		
		$submitCategory = mysql_real_escape_string($submitCategory);
		$userID = mysql_real_escape_string($userID);
		$reSubmit = mysql_real_escape_string($reSubmit);
		$submitName = mysql_real_escape_string(strtolower($submitName)); //handles SQL injection
		
		//check if name already exists
		$result = mysql_query("SELECT name FROM `items` WHERE `topicID` = '$submitCategory' AND `name` = '$submitName'") or die(mysql_error()); 
		$row = mysql_fetch_array($result);
		
		$returnVars['didYouMean'] = "false";
		
		if ($row == false){
		//if it doesn't exist
			
			$submitNameParsed = str_replace(" ","%20",$submitName);
			
			
			
			//2010-01-17, Tal, commented out Did You Mean func.
			
			/*
			//Yahoo API service request (currently using Itai's private yahoo key. This should be changed into a public IBM key)
			$request = "http://search.yahooapis.com/WebSearchService/V1/spellingSuggestion?appid=wgHAtVPV34GAhIe46z1h7WQfNeYOugC8wSoGY24Ex71ILTAxWjVhUgc9.Kb5dA&query=".$submitNameParsed."&output=php";
			
			
			$phpserialized = file_get_contents($request);
			// Parse the serialized response
			$phparray = unserialize($phpserialized);
			
			$didYouMeanResult = $phparray['ResultSet']['Result'];
			
			//$result3 = mysql_query("SELECT name FROM `items` WHERE SOUNDEX('$submitName') LIKE SOUNDEX(name) AND `topicID` = '$submitCategory' LIMIT 1 ") or die(mysql_error()); 
			//$row3 = mysql_fetch_array($result3);
			
			if ($didYouMeanResult == "" || $reSubmit == "true"){
			*/ if (1) { // 
			
				//add name to list
				mysql_query("INSERT INTO `items` (name, topicID) VALUES ('$submitName', '$submitCategory')");
				//add user to itemsmentions
				mysql_query("INSERT INTO `itemsmentions` (userID, itemID, topicID) VALUES ('$userID', (SELECT id FROM `items` WHERE name = '$submitName' AND `topicID` = '$submitCategory'), '$submitCategory')");
				//mysql_query("INSERT INTO `itemsmentions` (userID, itemID, topicID) VALUES ('test', '1', $submitCategory')");
				$returnVars['submitCategory'] = "You are the first one to mention this name";
				
				
			} else {
				$returnVars['submitCategory'] = $didYouMeanResult; //$row3['name'];
				$returnVars['didYouMean'] = "true";
			}
		
		} else {
		//if it does exist
			//add user to itemmentions
			//Check if the user hasn't said this name already
			$result2 = mysql_query("SELECT * FROM `itemsmentions` WHERE `topicID` = '$submitCategory' AND `itemID` = (SELECT id FROM `items` WHERE name = '$submitName' AND `topicID` = '$submitCategory') AND `userID` = '$userID'");
			$row2 = mysql_fetch_array($result2);
			
			if ($row2 == false){
				mysql_query("INSERT INTO `itemsmentions` (userID, itemID, topicID) VALUES ('$userID', (SELECT id FROM `items` WHERE name = '$submitName' AND `topicID` = '$submitCategory'), '$submitCategory')");


				
				//======================================================
				//-----------------Count mentions-----------------------
				$mentionCount = mysql_query("SELECT COUNT(*) FROM `itemsmentions` WHERE `itemID` = (SELECT id FROM `items` WHERE name = '$submitName' AND `topicID` = '$submitCategory')");
				$mentionCountResults = mysql_fetch_array($mentionCount);
				
				$numberOfPeople = ($mentionCountResults['COUNT(*)']-1);
				
				if ($numberOfPeople == 1	){
					$returnVars['submitCategory'] = "1 other person previously mentioned this name";
				} else {
					$returnVars['submitCategory'] = $numberOfPeople." other people previously mentioned this name";
				}
				//======================================================
				
				//======================================================
				//--------------send points to users--------------------
				$listMentionedBefore = mysql_query("SELECT userID FROM `itemsmentions` WHERE `itemID` = (SELECT id FROM `items` WHERE name = '$submitName' AND `topicID` = '$submitCategory') AND userID <> '$userID' ORDER BY time ASC"); //
						
				$pointsDistributed = 0;
				$pointsSelf = 0;

				$scoreShift = 5;

				if (($mentionCountResults['COUNT(*)'] <=1) || ($mentionCountResults['COUNT(*)'] >= 10)) {
					$pointsTotal = 0;
				} else {
					$pointsTotal = floor(50/pow(2,abs($mentionCountResults['COUNT(*)']-$scoreShift)));
				}
				
				$originalPointsTotal = $pointsTotal;


				if (($pointsTotal > 0) && ($pointsTotal < 1)){
						$pointsTotal = 1;	
				}

                $pointsCount = $pointsTotal;
                $pointsSelf = floor($pointsTotal/pow(2,abs($mentionCountResults['COUNT(*)']-$scoreShift)));

				while($addPoints = mysql_fetch_array($listMentionedBefore)){

					//$pointsCount = floor($pointsTotal/pow(2,($pointsDistribute)));
					//$pointsSelf = floor($pointsTotal/pow(2,($mentionCountResults['COUNT(*)']-1)));

                    $pointsCount = $pointsTotal;
					$pointsTotal = $pointsTotal * 0.75;

					if ($pointsSelf < 1){
						$pointsSelf = 1;	
					}
					if ($pointsCount < 1){
						$pointsCount = 1;	
					}
		
					$userToUpdate = $addPoints['userID'];
					mysql_query("UPDATE `users` SET dividendPoints = dividendPoints+'$pointsCount' WHERE userID = '$userToUpdate'");
					
					$result = mysql_query("SELECT * FROM `users` WHERE `userID` = 'ido@il.ibm.com' AND `time` BETWEEN TIMESTAMPADD(MINUTE,-2,NOW()) AND NOW()");
					$row = mysql_fetch_array($result);
					if ($row2 == false){
						mysql_query("UPDATE `users` SET pointsSinceLastLogin = pointsSinceLastLogin+'$pointsCount' WHERE userID = '$userToUpdate'");
					}
					
					$pointsDistributed = $pointsDistributed + 1;
				}
				
				if ($originalPointsTotal > 0) {
					$pointsSelf = $pointsTotal*2;
				} else {
					$pointsSelf = $pointsTotal;
				}
				mysql_query("UPDATE `users` SET Points = Points + '$pointsSelf' WHERE userID = '$userID'");
				
			} else {
				$returnVars['submitCategory'] = "selfMentioned";		
			}
		}
		
		$returnVars['itemsList'] = getNewGraph($submitCategory,$userID);
		$returnVars['functionSent'] = "checkNameSubmission";
		$returnVars['categoryID'] = $submitCategory;
		break;	
//---------------------------------------------		
	case 'getUpdatedItemList':
		$submitCategory = $_POST['categoryID'];
		$userID = $_POST['userID'];
		
		$submitCategory = mysql_real_escape_string($submitCategory);
		$userID = mysql_real_escape_string($userID);
		
		$returnVars['itemsList'] = getNewGraph($submitCategory,$userID);
		$returnVars['functionSent'] = "getUpdatedItemList";
		break;	
//---------------------------------------------		
} //end of switch function


//Wrap up of array and echo of that array back to the front-end actionscript
$returnString = http_build_query($returnVars);
echo $returnString;



//---------------------------------------------	
/*

*/
//Counts the number of players online. Returns an integer
function playersOnline(){
		$result = mysql_query("SELECT COUNT(*) FROM users WHERE time > DATE_ADD(NOW(), INTERVAL -2 MINUTE)");  
		$row = mysql_fetch_array($result);
		return $row['COUNT(*)'];
}

//Counts the number of playesr in the last seven days. Returns an integer.
function playersLast7Days(){
		$result = mysql_query("SELECT COUNT(*) FROM users WHERE time > DATE_ADD(NOW(), INTERVAL -7 DAY)");  
		$row = mysql_fetch_array($result);
		return $row['COUNT(*)'];
}

function getNewGraph($submitCategory,$userID){
		$itemList = "";

		$result = mysql_query("SELECT id,name FROM `items` WHERE `topicID` = '$submitCategory'"); 
		
		while($row = mysql_fetch_array($result)){
				$myitem = $row['id'];
				$myNames = mysql_query("SELECT * FROM `itemsmentions` WHERE `userID` = '$userID' AND `itemID` = '$myitem'");
				$myRow = mysql_fetch_array($myNames);

				if ($myRow == false){
					//if user hasn't said item, count number of people who said and add to list with unknown
					$mentionCount = mysql_query("SELECT COUNT(*) FROM `itemsmentions` WHERE `itemID` = '$myitem'");
					$mentionCountResults = mysql_fetch_array($mentionCount);
					
					if ($mentionCountResults['COUNT(*)'] > 10) {
						$mentionCountEdited = 10;
					} else {
						$mentionCountEdited = $mentionCountResults['COUNT(*)'];
					}
					if ($mentionCountResults['COUNT(*)'] > 1){
						$itemList = $itemList."&".$mentionCountEdited."=empty";
					}
				} else {
					//if user already set item, add it to list with 11
					
					//replace & sign with %26
					$mentioned = str_replace("=", "%25", str_replace("&", "%26", $row['name']));					
					
					
					$itemList = $itemList."&11=".$mentioned;	
				}			
		}
		return $itemList;	
}

?>