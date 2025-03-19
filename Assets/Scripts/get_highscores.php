<?php
ini_set('display_errors', 1);
ini_set('display_startup_errors', 1);
error_reporting(E_ALL);

$servername = "scoredb.c1ssmegceydx.eu-central-1.rds.amazonaws.com";
$username = "admin";
$password = "Yannick1";
$dbname = "Highscores";

// Verbindung zur Datenbank herstellen
$conn = new mysqli($servername, $username, $password, $dbname);

// Verbindung prüfen
if ($conn->connect_error) {
    die("Verbindung fehlgeschlagen: " . $conn->connect_error);
}

// Top-3-Highscores abfragen, sortiert nach Score (absteigend)
$sql = "SELECT name, score FROM SCORES ORDER BY score DESC LIMIT 3";
$result = $conn->query($sql);

// Ergebnisse in ein Array speichern
$highscores = array();
if ($result->num_rows > 0) {
    while ($row = $result->fetch_assoc()) {
        $highscores[] = $row;
    }
}

// JSON-Antwort senden
header('Content-Type: application/json');
echo json_encode($highscores);

// Verbindung schließen
$conn->close();
?>