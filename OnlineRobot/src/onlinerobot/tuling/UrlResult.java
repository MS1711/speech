package onlinerobot.tuling;

public class UrlResult extends CommonResult {
	private int code;
	private String text;
	private String url;
	public int getCode() {
		return code;
	}
	public String getText() {
		return text;
	}
	public String getUrl() {
		return url;
	}
	public void setCode(int code) {
		this.code = code;
	}
	public void setText(String text) {
		this.text = text;
	}
	public void setUrl(String url) {
		this.url = url;
	}
}
