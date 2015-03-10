package onlinerobot.tuling;

import java.util.List;
class Train{
	private String trainnum;
	private String start;
	private String terminal;
	private String starttime;
	private String endtime;
	private String detailurl;
	public String getDetailurl() {
		return detailurl;
	}
	public void setDetailurl(String detailurl) {
		this.detailurl = detailurl;
	}
	public String getIcon() {
		return icon;
	}
	public void setIcon(String icon) {
		this.icon = icon;
	}
	private String icon;
	public String getEndtime() {
		return endtime;
	}
	public String getStart() {
		return start;
	}
	public String getStarttime() {
		return starttime;
	}
	public String getTerminal() {
		return terminal;
	}
	public String getTrainnum() {
		return trainnum;
	}
	public void setEndtime(String endtime) {
		this.endtime = endtime;
	}
	public void setStart(String start) {
		this.start = start;
	}
	public void setStarttime(String starttime) {
		this.starttime = starttime;
	}
	public void setTerminal(String terminal) {
		this.terminal = terminal;
	}
	public void setTrainnum(String trainnum) {
		this.trainnum = trainnum;
	}
}
public class TrainResult extends CommonResult {
	private int code;
	private String text;
	private List<Train> list;
	public int getCode() {
		return code;
	}
	public List<Train> getList() {
		return list;
	}
	public String getText() {
		return text;
	}
	public void setCode(int code) {
		this.code = code;
	}
	public void setList(List<Train> list) {
		this.list = list;
	}
	public void setText(String text) {
		this.text = text;
	}
}
