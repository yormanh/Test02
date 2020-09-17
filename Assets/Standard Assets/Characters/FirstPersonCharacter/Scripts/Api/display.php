<?php

	include('config.php');

	$lsQuery = "select MAX(Score) AS Score, Name  FROM Test_Score group by Name order by Score desc limit 50";

	$resul = $conn->query($lsQuery);

	while ($lItem = $resul->fetch_object())
	{
		echo $lItem->Name ;
		echo ': ';
		echo $lItem->Score;
		echo ';';

	}

	
	
?>


