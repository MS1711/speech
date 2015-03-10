package onlinerobot.tuling;

import java.util.List;
class Film{
	private String name;
	private String info;
	private String detailurl;
	private String icon;
	public String getDetailurl() {
		return detailurl;
	}
	public String getIcon() {
		return icon;
	}
	public String getInfo() {
		return info;
	}
	public String getName() {
		return name;
	}
	public void setDetailurl(String detailurl) {
		this.detailurl = detailurl;
	}
	public void setIcon(String icon) {
		this.icon = icon;
	}
	public void setInfo(String info) {
		this.info = info;
	}
	public void setName(String name) {
		this.name = name;
	}
}
public class FilmResult extends CommonResult {
	private int code;
	private String text;
	private List<Film> list;
	public int getCode() {
		return code;
	}
	public List<Film> getList() {
		return list;
	}
	public String getText() {
		return text;
	}
	public void setCode(int code) {
		this.code = code;
	}
	public void setList(List<Film> list) {
		this.list = list;
	}
	public void setText(String text) {
		this.text = text;
	}
}
