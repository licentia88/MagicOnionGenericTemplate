<?php


use PHPMailer\PHPMailer\PHPMailer;


if($_POST)
{


    require_once "PHPMailer/Exception.php";
    require_once "PHPMailer/PHPMailer.php";
    require_once "PHPMailer/SMTP.php";

    $mail = new PHPMailer();

    $your_email = "youremail@website.com";


    //check if its an ajax request, exit if not
    if(!isset($_SERVER['HTTP_X_REQUESTED_WITH']) AND strtolower($_SERVER['HTTP_X_REQUESTED_WITH']) != 'xmlhttprequest') {

        //exit script outputting json data
        $output = json_encode(
            array(
                'type'=>'error',
                'text' => 'Request must come from Ajax'
            ));

        die($output);
    }

    //check $_POST vars are set, exit if any missing
    //Sanitize input data using PHP filter_var().

    if(isset($_POST["userName"])) {
        if(!isset($_POST["userName"]))
        {
            $output = json_encode(array('type'=>'error', 'text' => 'Input fields are empty!'));
            die($output);
        }
        else {
            $user_Name = filter_var($_POST["userName"], FILTER_SANITIZE_STRING);
        }
    }
    if(isset($_POST["firstName"]) && isset($_POST["lastName"])) {
        if(!isset($_POST["firstName"]) && !isset($_POST["lastName"]))
        {
            $output = json_encode(array('type'=>'error', 'text' => 'Input fields are empty!'));
            die($output);
        }
        else {
            $user_Name = filter_var($_POST["firstName"], FILTER_SANITIZE_STRING) . " " . filter_var($_POST["lastName"], FILTER_SANITIZE_STRING);
        }
    }
    //education
    if(isset($_POST["fatherName"])) {
        if(!isset($_POST["fatherName"]))
        {
            $output = json_encode(array('type'=>'error', 'text' => 'Input fields are empty!'));
            die($output);
        }
        else {
            $father_Name = filter_var($_POST["fatherName"], FILTER_SANITIZE_STRING);
        }
    }
    if(isset($_POST["quoteName"])) {
        if(!isset($_POST["quoteName"]))
        {
            $output = json_encode(array('type'=>'error', 'text' => 'Input fields are empty!'));
            die($output);
        }
        else {
            $quote_Name = filter_var($_POST["quoteName"], FILTER_SANITIZE_STRING);
        }
    }
    if(isset($_POST["userAddress"])) {
        if(!isset($_POST["userAddress"]))
        {
            $output = json_encode(array('type'=>'error', 'text' => 'Input fields are empty!'));
            die($output);
        }
        else {
            $user_Address = filter_var($_POST["userAddress"], FILTER_SANITIZE_STRING);
        }
    }
    if(isset($_POST["course"])) {
        if(!isset($_POST["course"]))
        {
            $output = json_encode(array('type'=>'error', 'text' => 'Input fields are empty!'));
            die($output);
        }
        else {
            $applied_Course = filter_var($_POST["course"], FILTER_SANITIZE_STRING);
        }
    }

    if(isset($_POST["userEmail"])) {
        if(!isset($_POST["userEmail"]))
        {
            $output = json_encode(array('type'=>'error', 'text' => 'Input fields are empty!'));
            die($output);
        }
        else {
            $user_Email = filter_var($_POST["userEmail"], FILTER_SANITIZE_EMAIL);
        }
    }
    if(isset($_POST["userPhone"])){
        if(!isset($_POST["userPhone"]))
        {
            $output = json_encode(array('type'=>'error', 'text' => 'Input fields are empty!'));
            die($output);
        }
        else {
            $user_Phone = $_POST["userPhone"];
        }
    }
    if(isset($_POST["userSubject"])) {
        if(!isset($_POST["userSubject"]))
        {
            $output = json_encode(array('type'=>'error', 'text' => 'Input fields are empty!'));
            die($output);
        }
        else {
            $user_Subject = $_POST["userSubject"];
        }
    }
    if(isset($_POST["userCity"])) {
        if(!isset($_POST["userCity"]))
        {
            $output = json_encode(array('type'=>'error', 'text' => 'Input fields are empty!'));
            die($output);
        }
        else {
            $user_City = $_POST["userCity"];
        }
    }
    if(isset($_POST["projectType"])) {
        if(!isset($_POST["projectType"]))
        {
            $output = json_encode(array('type'=>'error', 'text' => 'Input fields are empty!'));
            die($output);
        }
        else {
            $pro_type = $_POST["projectType"];
        }
    }

    //Directory listing
    if(isset($_POST["propertyId"])) {
        if(!isset($_POST["propertyId"]))
        {
            $output = json_encode(array('type'=>'error', 'text' => 'Input fields are empty!'));
            die($output);
        }
        else {
            $property_id = $_POST["propertyId"];
        }
    }
    if(isset($_POST["propertyType"])) {
        if(!isset($_POST["propertyType"]))
        {
            $output = json_encode(array('type'=>'error', 'text' => 'Input fields are empty!'));
            die($output);
        }
        else {
            $property_type = $_POST["propertyType"];
        }
    }
    if(isset($_POST["quoteBudget"])) {
        if(!isset($_POST["quoteBudget"]))
        {
            $output = json_encode(array('type'=>'error', 'text' => 'Input fields are empty!'));
            die($output);
        }
        else {
            $budget = $_POST["quoteBudget"];
        }
    }
    //consultant template
    if(isset($_POST["service"])) {
        if(!isset($_POST["service"]))
        {
            $output = json_encode(array('type'=>'error', 'text' => 'Input fields are empty!'));
            die($output);
        }
        else {
            $service = $_POST["service"];
        }
    }
    //Reservation template
    if(isset($_POST["reservationDate"])) {
        if(!isset($_POST["reservationDate"]))
        {
            $output = json_encode(array('type'=>'error', 'text' => 'Input fields are empty!'));
            die($output);
        }
        else {
            $res_date = $_POST["reservationDate"];
        }
    }
    if(isset($_POST["totalPeople"])) {
        if(!isset($_POST["totalPeople"]))
        {
            $output = json_encode(array('type'=>'error', 'text' => 'Input fields are empty!'));
            die($output);
        }
        else {
            $total_people = $_POST["totalPeople"];
        }
    }
    //spa
    if(isset($_POST["reserveTime"])) {
        if(!isset($_POST["reserveTime"]))
        {
            $output = json_encode(array('type'=>'error', 'text' => 'Input fields are empty!'));
            die($output);
        }
        else {
            $res_time = $_POST["reserveTime"];
        }
    }

    //medical
    if(isset($_POST["userGender"])) {
        if(!isset($_POST["userGender"]))
        {
            $output = json_encode(array('type'=>'error', 'text' => 'Input fields are empty!'));
            die($output);
        }
        else {
            $user_gender = $_POST["userGender"];
        }
    }
    if(isset($_POST["userMessage"])) {
        if(!isset($_POST["userMessage"]))
        {
            $output = json_encode(array('type'=>'error', 'text' => 'Input fields are empty!'));
            die($output);
        }
        else {
            $user_Message = filter_var($_POST["userMessage"], FILTER_SANITIZE_STRING);
        }
    }


    //additional php validation
    if(isset($user_Name)) {
        if (strlen($user_Name) < 3) // If length is less than 3 it will throw an HTTP error.
        {
            $output = json_encode(array('type' => 'error', 'text' => 'Name is too short or empty!'));
            die($output);
        }
    }
    if(isset($_POST["userEmail"])) {
        if (!filter_var($user_Email, FILTER_VALIDATE_EMAIL)) //email validation
        {
            $output = json_encode(array('type' => 'error', 'text' => 'Please enter a valid email!'));
            die($output);
        }
    }
    if(isset($_POST["userMessage"])) {
        if (strlen($user_Message) < 5) //check emtpy message
        {
            $output = json_encode(array('type' => 'error', 'text' => 'Too short message! Please enter something.'));
            die($output);
        }
    }



    //Server settings
//    $mail->isSMTP();                                            // Send using SMTP
//    $mail->Host       = 'smtp.googlemail.com';                    // Set the SMTP server to send through
//    $mail->SMTPAuth   = true;                                   // Enable SMTP authentication
//    $mail->Username   = 'website@gmail.com';                     // SMTP username
//    $mail->Password   = 'your password';                         // SMTP password
//    $mail->SMTPSecure = 'TLS';         // Enable TLS encryption; `PHPMailer::ENCRYPTION_SMTPS` also accepted
//    $mail->Port       = 587;                                    // TCP port to connect to

    //Recipients
    $mail->setFrom($user_Email,$user_Name);
    $mail->addAddress($your_email, 'Theme Industry');     // Add a recipient
    $mail->addReplyTo($your_email, 'Information');


    // Content
    $mail->isHTML(true);                                  // Set email format to HTML
    $mail->Subject = 'New Contact Inquiry from your Website';
    $mail->Body  = "<h4 style='text-align: center;padding: 25px 15px;background-color: #0c6c9e;color: #FFFFFF;font-size:16px;width:90%;border-radius: 10px;'>Hi There! You have a new inquiry from your website.</h4><br><br>";

    if(isset($_POST["userEmail"])) {
        $mail->Body .= "<strong>Email: </strong>" . $user_Email . "<br>";
    }
    //education
    if(isset($_POST["fatherName"])) {
        $mail->Body .= "<strong>Father Name: </strong>" . $father_Name . "<br>";
    }
    if(isset($_POST["userAddress"])) {
        $mail->Body .= "<strong>Address: </strong>" . $user_Address . "<br>";
    }
    if(isset($_POST["course"])) {
        $mail->Body .= "<strong>Applied Course: </strong>" . $applied_Course . "<br>";
    }
    if(isset($_POST["userPhone"])) {
        $mail->Body .= "<strong>Phone: </strong>" . $user_Phone . "<br>";
    }
    if(isset($_POST["userSubject"])) {
        $mail->Body .= "<strong>Subject: </strong>" . $user_Subject . "<br>";
    }
    if(isset($_POST["userCity"])) {
        $mail->Body .= "<strong>City Or Country: </strong>" . $user_City . "<br>";
    }
    if(isset($_POST["projectType"])) {
        $mail->Body .= "<strong>Project Type: </strong>" . $pro_type . "<br>";
    }
    if(isset($_POST["quoteBudget"])) {
        $mail->Body .= "<strong>Budget: </strong>" . $budget . "<br>";
    }
    //Directory listing
    if(isset($_POST["propertyId"])) {
        $mail->Body .= "<strong>Property Id: </strong>" . $property_id . "<br>";
    }
    if(isset($_POST["propertyType"])) {
        $mail->Body .= "<strong>Property Type: </strong>" . $property_type . "<br>";
    }
    // dental
    if(isset($_POST["service"])) {
        $mail->Body .= "<strong>Service Type: </strong>" . $service . "<br>";
    }
    //Reservation , spa , medical template
    if(isset($_POST["reservationDate"])) {
        $mail->Body .= "<strong>Reservation Date: </strong>" . $res_date . "<br>";
    }
    //spa
    if(isset($_POST["reserveTime"])) {
        $mail->Body .= "<strong>Reserved Time: </strong>" . $res_time . "<br>";
    }
    if(isset($_POST["totalPeople"])) {
        $mail->Body .= "<strong>Total People: </strong>" . $total_people . "<br>";
    }
    //medical
    if(isset($_POST["userGender"])) {
        $mail->Body .= "<strong>Gender: </strong>" . $user_gender . "<br>";
    }
    $mail->Body .= '<br>';

    if(isset($_POST["userMessage"])) {
        $mail->Body .= "<strong>Message: </strong><br><br><div style='background-color: #EDEFF2;padding:30px 15px;border-radius:10px;min-height:50px;width:90%;'>" . $user_Message . "</div><br>";
    }
    $mail->Body .= '<strong>Best Regards,</strong><br>';

    if(isset($user_Name)) {
        $mail->Body .= $user_Name . "<br>";
    }
    if(isset($_POST["quoteName"])) {
        $mail->Body .= "<strong>Quote Name: </strong>" . $quote_Name . "<br>";
    }
    $mail->AltBody = 'This is the body in plain text for non-HTML mail clients';


    if(!$mail->send())
    {
        $output = json_encode(array('type'=>'error', 'text' => 'Could not send mail! Please check your PHP mail configuration.'));
        die($output);
    }else{
        $output = json_encode(array('type'=>'message', 'text' => 'Hi '.$user_Name .' Thank you for contacting us.'));
        die($output);
    }
}
?>