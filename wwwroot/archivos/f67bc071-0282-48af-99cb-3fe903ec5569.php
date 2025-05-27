<?php

session_start();

$error=array();
if(isset($_POST['nombre'])) $nombre=$_POST['nombre']; else $nombre="";
if(isset($_POST['apellido'])) $apellido=$_POST['apellido']; else $apellido="";
if(isset($_POST['telefono'])) $telefono=$_POST['telefono']; else $telefono="";
if(isset($_POST['ciudad'])) $ciudad=$_POST['ciudad']; else $ciudad="";
if(isset($_POST['email'])) $email=$_POST['email']; else $email="";
if(isset($_POST['mensaje'])) $mensaje=$_POST['mensaje']; else $mensaje="";

$error_nombre="Ingrese el nombre";
$error_email1="Ingrese el email";
$error_email2="Ingrese un e-mail correcto";
$error_mensaje="Ingrese el mensaje";

$error_email=false;

if(isset($_POST['accion']))
{
    if(!$nombre)
        $error['nombre']=$error_nombre;
    if(!$email)
        $error['email']=$error_email1;
    elseif( !preg_match("/^([a-z0-9\+_\-]+)(\.[a-z0-9\+_\-]+)*@([a-z0-9\-]+\.)+[a-z]{2,6}$/ix", $email))
        $error['email']=$error_email2;
    if(!$mensaje)
        $error['mensaje']=$error_mensaje;
 
    if(count($error)==0)
    {
        @ini_set(sendmail_from,"info@sdg-norton.com.ar");

        $headers = "MIME-Version: 1.0\n";
        $headers .= "Content-type: text/html; charset=utf-8\n";
        $headers .= "From: ".$email." (Estudio Pedernera)\n";
        $headers .= "Reply-To: ".$email."\n";

        $msg  = '';
        $msg .= 'Nombre: ' . utf8_encode($nombre) . '<br><br>';	
        $msg .= 'Apellido: ' . utf8_encode($apellido) . '<br><br>';	
        $msg .= 'E-mail: ' . utf8_encode($email) . '<br><br>';
        $msg .= 'Ciudad: ' . utf8_encode($ciudad) . '<br><br>';
        $msg .= 'Tel&eacute;fono: ' . utf8_encode($telefono) . '<br><br>';
        $msg .= 'Mensaje:<br>' . nl2br(utf8_encode($mensaje)) ;
        @ini_set(sendmail_from,"info@sdg-norton.com.ar");
        
        $success = @mail("info@sdg-norton.com.ar", "Contacto desde Formulario", $msg, $headers);
        
        if ($success)
        {
            session_register('contact_ok'); 
            $_SESSION['contact_ok']=TRUE;
            header('Location: '.$_SERVER['REQUEST_URI']);
            exit();
        }
        else
            $error_email=TRUE;
    }

}


?>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<meta http-equiv="Content-Type" content="text/html; charset=iso-8859-2" />
<title>Contacto - SDG.Norton - Soluciones de gesti&oacute;n</title>
<meta name="keywords" content="Soluciones de gestion, Contacto, teléfonos, teléfono, e-mail, email, dirección." />

<link href="estilos.css" rel="stylesheet" type="text/css" />
<script src="http://ajax.googleapis.com/ajax/libs/jquery/1.4.2/jquery.min.js" type="text/javascript"></script>
<script src="js/jquery.validate.js" type="text/javascript"></script>
<script type="text/javascript">
$(document).ready(function() {
    $("#contacto").validate({
        rules:
        {
            'nombre': { required: true },
            'email': { required: true, email: true },
            'mensaje': { required: true }
        },
        messages:
        {
            'nombre': { required: "<?php echo $error_nombre; ?>" },
            'email': { required: "<?php echo $error_email1; ?>", email: "<?php echo $error_email2; ?>" },
            'mensaje': { required: "<?php echo $error_mensaje; ?>" }
        }
    });
});
</script>
</head>

<body>
<table width="745" border="0" align="center" cellpadding="0" cellspacing="0" class="border">
  <tr>
    <td width="745" height="172" align="right" valign="bottom" background="img/top.jpg"><table width="300" height="116" border="0" cellpadding="0" cellspacing="15">
      <tr>
        <td align="right" valign="top" class="titulos2">San Lorenzo N&deg;12, piso 1 oficina 1<br />
          Ciudad - Mendoza - CP(5500)<br />
          Tel-Fax: +54 9 2615 16-6023</td>
      </tr>
    </table>
      <table width="493" border="0" align="right" cellpadding="0" cellspacing="0">
        <tr>
          <td width="97" height="35" align="center" valign="middle"><a href="nosotros.html" class="botones">nosotros</a></td>
          <td width="97" align="center" valign="middle"><a href="servicios.html" class="botones">servicios</a></td>
          <td width="97" align="center" valign="middle"><a href="mercosur.html" class="botones">mercosur</a></td>
          <td width="97" align="center" valign="middle" class="botones"><h1>contactos</h1></td>
          <td width="97" align="center" valign="middle"><a href="links.html" class="botones">links</a></td>
          <td width="8">&nbsp;</td>
        </tr>
        <tr>
          <td colspan="6">&nbsp;</td>
        </tr>
      </table></td>
  </tr>
  <tr>
    <td width="745" height="380" align="left" valign="top" background="img/contacto-bg.jpg"><?php if($error_email) echo "<p>$error_email</p>"; ?>
      <?php if(isset($_SESSION['contact_ok']) ) { unset($_SESSION['contact_ok']); session_destroy(); $nombre=$apellido=$ciudad=$email=$telefono=$mensaje;
echo '<p class="msgOk">Su consulta ha sido enviada correctamente</p>';
} elseif($error_email) { echo '<p class="msgErr">Hubo un error al enviar el mensaje, vuelva a intentar en unos minutos</p>'; } ?>
      <table width="740" border="0" cellspacing="15" cellpadding="0">
      <tr>
        <td width="237" valign="top"><p> <span class="texto1">Dr. Oscar Alejandro Norton</span><br />
            <span class="texto2">Contador P&uacute;blico<br />
            Especialista en sindicatura concursal y entes en insolvencia<br />
            Especialista en gesti&oacute;n de <br />
            servicios de salud</span></p>
          <p class="texto1">Tel-Fax: +54 9 2615 16-6023<br />
          San Lorenzo N&deg;12, piso 1 oficina 1 Ciudad, Mendoza, Argentina<br />
          info@sdg-norton.com.ar</p></td>
        <td width="458" valign="top"><form id="contacto" name="contacto" method="post" action="">
          <table width="458" border="0" align="right" cellpadding="0" cellspacing="5">
            <tr>
              <td valign="top" width="74" class="fuente-formulario">Nombre:</td>
              <td width="377"><label for="nombre"></label>
                <input name="nombre" type="text" class="form-campo" id="nombre" value="<?php echo $nombre; ?>" />
                <?php if(isset($error['nombre']) && $error['nombre']) { echo '<label class="error-message" for="nombre" generated="true">'.$error['nombre'].'</label>'; } ?></td>
            </tr>
            <tr>
              <td height="10" colspan="2"></td>
            </tr>
            <tr>
              <td valign="top" class="fuente-formulario">Apellido:</td>
              <td><input name="apellido" type="text" class="form-campo" id="apellido" value="<?php echo $apellido; ?>" /></td>
            </tr>
            <tr>
              <td height="10" colspan="2"></td>
            </tr>
            <tr>
              <td valign="top" class="fuente-formulario">E-Mail:</td>
              <td><input name="email" type="text" class="form-campo" id="email" value="<?php echo $email; ?>" />
                <?php if(isset($error['email']) && $error['email']) { echo '<label class="error-message" for="nombre" generated="true">'.$error['email'].'</label>';  } ?></td>
            </tr>
            <tr>
              <td height="10" colspan="2"></td>
            </tr>
            <tr>
              <td valign="top" class="fuente-formulario">Ciudad:</td>
              <td><input name="ciudad" type="text" class="form-campo" id="ciudad" value="<?php echo $ciudad; ?>" /></td>
            </tr>
            <tr>
              <td height="10" colspan="2"></td>
            </tr>
            <tr>
              <td valign="top" class="fuente-formulario">Tel&eacute;fono:</td>
              <td><input name="telefono" type="text" class="form-campo" id="telefono" value="<?php echo $telefono; ?>" /></td>
            </tr>
            <tr>
              <td height="10" colspan="2"></td>
            </tr>
            <tr>
              <td valign="top" class="fuente-formulario">Mensaje:</td>
              <td><textarea name="mensaje" class="form-campo-mensaje" id="mensaje"><?php echo $mensaje; ?></textarea>
                <?php if(isset($error['mensaje']) && $error['mensaje']) { echo '<label class="error-message" for="nombre" generated="true">'.$error['mensaje'].'</label>';  } ?></td>
            </tr>
            <tr>
              <td valign="top" class="fuente-formulario">&nbsp;</td>
              <td>&nbsp;</td>
            </tr>
            <tr>
              <td valign="top" class="fuente-formulario">&nbsp;</td>
              <td align="right"><input type="submit" name="accion" class="texto-marron" value="ENVIAR" /></td>
            </tr>
          </table>
        </form></td>
        </tr>
    </table></td>
  </tr>
  <tr>
    <td height="24" bgcolor="#F3752E"><div align="center">
      <table width="709" border="0" cellspacing="10" cellpadding="0">
        <tr>
          <td width="360" align="left" valign="top"><table width="350" border="0" cellspacing="0" cellpadding="0">
            <tr>
              <td valign="top"><a href="nosotros.html" class="text-blanco">Nosotros</a><br />
                <a href="mercosur.html" class="text-blanco">Mercosur</a><br />
                <a href="contactos.php" class="text-blanco">Contactos</a><br />
                <a href="links.html" class="text-blanco">Links</a></td>
              <td valign="top"><a href="servicios.html" class="text-blanco">Servicios</a><br />
                <a href="servicios-contable-auditoria.html" class="text-blanco"> &#8226; &aacute;rea contable y auditor&iacute;a</a><br />
                <a href="servicios-societaria.html" class="text-blanco">
&#8226; &aacute;rea societaria</a><br />
                <a href="servicios-recursos-humanos.html" class="text-blanco">
&#8226; &aacute;rea recursos humanos</a><br />
                <a href="servicios-impuestos.html" class="text-blanco">
&#8226; &aacute;rea impuestos</a><br />
                <a href="servicios-contable-auditoria.html" class="text-blanco">		&#8226; &aacute;rea sistemas</a><br />
                <a href="servicios-contable-auditoria.html" class="text-blanco">&#8226; &aacute;rea autsourcing</a></td>
            </tr>
          </table>
            <p>&nbsp;</p></td>
          <td width="122" valign="top"><img src="img/logo-ch.jpg" width="120" height="75" /></td>
          <td width="187" valign="top"><p class="text-blanco"> San Lorenzo N&deg;12, piso 1 oficina 1<br />
            Ciudad - Mendoza - CP(5500)<br />
            Tel-Fax: +54 9 2615 16-6023</p></td>
        </tr>
      </table>
    </div></td>
  </tr>
</table>
<script type="text/javascript">

  var _gaq = _gaq || [];
  _gaq.push(['_setAccount', 'UA-9092715-1']);
  _gaq.push(['_trackPageview']);

  (function() {
    var ga = document.createElement('script'); ga.type = 'text/javascript'; ga.async = true;
    ga.src = ('https:' == document.location.protocol ? 'https://ssl' : 'http://www') + '.google-analytics.com/ga.js';
    var s = document.getElementsByTagName('script')[0]; s.parentNode.insertBefore(ga, s);
  })();

</script>
</body>
</html>
