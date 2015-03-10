package onlinerobot.tuling;

import java.util.List;
class Flight{
	private String flight;
	private String route;
	private String starttime;
	private String endtime;
	private String state;
	private String detailurl;
	private String icon;
	public String getDetailurl() {
		return detailurl;
	}
	public String getEndtime() {
		return endtime;
	}
	public String getFlight() {
		return flight;
	}
	public String getIcon() {
		return icon;
	}
	public String getRoute() {
		return route;
	}
	public String getStarttime() {
		return starttime;
	}
	public String getState() {
		return state;
	}
	public void setDetailurl(String detailurl) {
		this.detailurl = detailurl;
	}
	public void setEndtime(String endtime) {
		this.endtime = endtime;
	}
	public void setFlight(String flight) {
		this.flight = flight;
	}
	public void setIcon(String icon) {
		this.icon = icon;
	}
	public void setRoute(String route) {
		this.route = route;
	}
	public void setStarttime(String starttime) {
		this.starttime = starttime;
	}
	public void setState(String state) {
		this.state = state;
	}
}
public class FlightResult extends CommonResult {
	private int code;
	private String text;
	private List<Flight> list;
	public int getCode() {
		return code;
	}
	public List<Flight> getList() {
		return list;
	}
	public String getText() {
		return text;
	}
	public void setCode(int code) {
		this.code = code;
	}
	public void setList(List<Flight> list) {
		this.list = list;
	}
	public void setText(String text) {
		this.text = text;
	}
}
