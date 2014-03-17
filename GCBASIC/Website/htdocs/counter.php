<?php

	if (file_exists('hits.txt')) 
	{
		$fil = fopen('hits.txt', r);
		$dat = fread($fil, filesize('hits.txt')); 
		echo $dat+1;
		fclose($fil);
		$fil = fopen('hits.txt', w);
		fwrite($fil, $dat+1);
	}

	else
	{
		$fil = fopen('hits.txt', w);
		fwrite($fil, 1);
		echo '1';
		fclose($fil);
	}
?>