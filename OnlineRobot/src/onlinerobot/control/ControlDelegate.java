package onlinerobot.control;

import java.net.URLEncoder;
import java.util.List;
import com.google.gson.Gson;
import onlinerobot.sobot.Sobot;
import onlinerobot.sobot.SobotResult;
import onlinerobot.tuling.TuLing;
import onlinerobot.xiaobing.XiaoBing;

@javax.jws.WebService(targetNamespace = "http://control.onlinerobot/", serviceName = "ControlService", portName = "ControlPort", wsdlLocation = "WEB-INF/wsdl/ControlService.wsdl")
public class ControlDelegate {

	onlinerobot.control.Control control = new onlinerobot.control.Control();

	public String GetSobot(String query) {
		return control.GetSobot(query);
	}

	public String GetBing(String comments) {
		return control.GetBing(comments);
	}

	public String GetTuLing(String query) {
		return control.GetTuLing(query);
	}

	public void main(String[] args) {
		control.main(args);
	}

}