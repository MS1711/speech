package onlinerobot.tuling;

import java.util.List;
class Hotel{
	private String name;
	private String price;
	private String satisfaction;
	private String count;
	private String detailurl;
	private String icon;
	public String getCount() {
		return count;
	}
	public String getDetailurl() {
		return detailurl;
	}
	public String getIcon() {
		return icon;
	}
	public String getName() {
		return name;
	}
	public String getPrice() {
		return price;
	}
	public String getSatisfaction() {
		return satisfaction;
	}
	public void setCount(String count) {
		this.count = count;
	}
	public void setDetailurl(String detailurl) {
		this.detailurl = detailurl;
	}
	public void setIcon(String icon) {
		this.icon = icon;
	}
	public void setName(String name) {
		this.name = name;
	}
	public void setPrice(String price) {
		this.price = price;
	}
	public void setSatisfaction(String satisfaction) {
		this.satisfaction = satisfaction;
	}
}

public class HotelResult extends CommonResult {
	private int code;
	private String text;
	private List<Hotel> list;
	public int getCode() {
		return code;
	}
	public List<Hotel> getList() {
		return list;
	}
	public String getText() {
		return text;
	}
	public void setCode(int code) {
		this.code = code;
	}
	public void setList(List<Hotel> list) {
		this.list = list;
	}
	public void setText(String text) {
		this.text = text;
	}
}
