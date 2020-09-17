<?php
		$dbHost = 'localhost';
		$dbUser = '155835';
		$dbPassword = 'rAL6qcj3fjcnrjB';
		$dbName = '155835';



		$conn = new mysqli($dbHost, $dbUser, $dbPassword);

		$conn->select_db($dbName);
		$conn->query("set character_set_server='utf8'");
		$conn->query("set name 'utf8'");

	
?>