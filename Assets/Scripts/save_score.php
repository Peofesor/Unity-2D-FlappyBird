<?php
ini_set('display_errors', 1);
ini_set('display_startup_errors', 1);
error_reporting(E_ALL);

$servername = "highscoredb.c1ssmegceydx.eu-central-1.rds.amazonaws.com";
$username = "admin";
$password = "Yannick1";
$dbname = "Highscores";

// Verbindung zur Datenbank herstellen
$conn = new mysqli($servername, $username, $password, $dbname);

// Verbindung prüfen
if ($conn->connect_error) {
    die("Verbindung fehlgeschlagen: " . $conn->connect_error);
}

// Prüfen, ob die POST-Daten gesetzt sind
if (!isset($_POST['player_name']) || !isset($_POST['score'])) {
    die("Fehler: player_name oder score fehlen!");
}

$player_name = $_POST['player_name'];
$score = (int)$_POST['score'];

// Prepared Statement verwenden, um SQL-Injection zu verhindern
$stmt = $conn->prepare("INSERT INTO scores (name, score, timestamp) VALUES (?, ?, NOW())");
$stmt->bind_param("si", $player_name, $score);

if ($stmt->execute()) {
    echo "Score erfolgreich gespeichert!";
} else {
    echo "Fehler: " . $stmt->error;
}

$stmt->close();
$conn->close();
?>