<?php

//http://yormanh.orgfree.com/test/addscore.php?liScore=200&lsName=Test04


	include('config.php');

	$liScore = $_GET['liScore']; 
	$lsName = $_GET['lsName'];

	//$liScore = $_POST['liScore']; 
	//$lsName = $_POST['lsName'];

	
	if (($liScore != NULL || $liScore != "") && ($lsName != NULL || $lsName != ""))
	{
		$lsQuery = "INSERT INTO Test_Score (Name, Score) VALUES ('$lsName', '$liScore')";  
		$conn->query($lsQuery);
	
	}

?>