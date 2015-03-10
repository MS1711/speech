package onlinerobot.tuling;

import java.util.List;

class News{
	private String article;
	private String source;
	private String detailurl;
	private String icon;
	public String getArticle() {
		return article;
	}
	public String getDetailurl() {
		return detailurl;
	}
	public String getIcon() {
		return icon;
	}
	public String getSource() {
		return source;
	}
	public void setArticle(String article) {
		this.article = article;
	}
	public void setDetailurl(String detailurl) {
		this.detailurl = detailurl;
	}
	public void setIcon(String icon) {
		this.icon = icon;
	}
	public void setSource(String source) {
		this.source = source;
	}
}
public class NewsResult extends CommonResult {
	private int code;
	private String text;
	private List<News> list;
	public int getCode() {
		return code;
	}
	public List<News> getList() {
		return list;
	}
	public String getText() {
		return text;
	}
	public void setCode(int code) {
		this.code = code;
	}
	public void setList(List<News> list) {
		this.list = list;
	}
	public void setText(String text) {
		this.text = text;
	}
}
