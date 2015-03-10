package onlinerobot.control;

public class TestFunction {

	/**
	 * @param args
	 */
	private String FiterResult(String result) {//过滤掉一些乱字符
		// TODO Auto-generated method stub
		StringBuilder afterFilter  = new StringBuilder("");
		afterFilter.append(result);
		int endIndex = -1;
		int beginIndex  = afterFilter.indexOf("<");
		while(beginIndex != -1){
			endIndex = afterFilter.indexOf(">");
			if(endIndex != -1 && beginIndex < endIndex)
				afterFilter.replace(beginIndex, endIndex+1, "");
			else if(endIndex != -1 && beginIndex > endIndex){
				afterFilter.replace(endIndex, endIndex + 1, "");
			}
			else
				afterFilter.replace(beginIndex, beginIndex + 1, "");
			beginIndex  = afterFilter.indexOf("<");
		}
		endIndex = afterFilter.indexOf(">");
		while(endIndex != -1){		
			afterFilter.replace(endIndex, endIndex + 1, "");
			endIndex = afterFilter.indexOf(">");
		}
		return afterFilter.toString();
	}
	public static void main(String[] args) {
		// TODO Auto-generated method stub
		TestFunction test = new TestFunction();
		System.out.println(test.FiterResult("<brsfjafa<br>"));
	}

}
