package onlinerobot.tuling;

import java.util.List;
class Price{
	private String name;
	private String price;
	private String detailurl;
	private String icon;
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
}
public class PriceResult {
	private int code;
	private String text;
	private List<Price> list;
	public int getCode() {
		return code;
	}
	public List<Price> getList() {
		return list;
	}
	public String getText() {
		return text;
	}
	public void setCode(int code) {
		this.code = code;
	}
	public void setList(List<Price> list) {
		this.list = list;
	}
	public void setText(String text) {
		this.text = text;
	}
	
}
