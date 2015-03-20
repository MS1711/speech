/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package NLPServer;
import getBaiduMusic.SearchMusic;

import java.io.BufferedReader;
import java.io.File;
import java.io.FileReader;
import java.io.IOException;
import java.util.ArrayList;
import java.util.Calendar;
import java.util.Date;
import java.util.Hashtable;
import java.util.logging.Level;
import java.util.logging.Logger;
import java.util.regex.Matcher;
import java.util.regex.Pattern;

import onlinerobot.control.ControlDelegate;
import onlinerobot.control.ControlService;

import org.fnlp.nlp.cn.CNFactory;
import org.fnlp.util.exception.LoadModelException;

import properties.Config;
import weather.api.WeatherService;
import ActiveMQ.messageQueue;
import LoadLocalSource.GetSource;
import SearchService.DBSearch;

import com.github.stuxuhai.jpinyin.ChineseToPinyin;

import device.control.DeviceControlProxy;


/**
 *
 * @author v-chenfei
 */
public class NLPCore {
    ControlService control = null;
    ControlDelegate ControlDelegate = null;
    CNFactory factory = null;
    messageQueue mq; 
    GetSource getSource;
    SearchMusic getMusic;
    ArrayList<String> noResultList;
    int noResultIndex = 0;
    DBSearch dbSearcher;
    ChineseToPinyin ctp;
    ArrayList<String> deviceType;
	private boolean inited = false;
	String dictPath;
	
    public static void main(String[] args)  {
    	String[] words = new String[]{
//    			"$今天天气如何",
//    			"$北京明天天气如何",
//    			"$北京后天天气如何",
//    			"$北京3月7号天气如何",
//    			"$北京3月7号到3月10号天气如何",
//    			
//    			"$打开客厅的灯",
//    			"$关闭客厅的灯",
//    			"$打开空气净化器",
//    			"$关闭空气净化器",
//    			"$查询室内空气状况",  
//    			
//    			"$播放李荣浩的模特",
//    			"$打开王蓉两个人的罪",
//    			"$我想听周华健的歌",
//    			"$播放摇滚音乐",
    			"$播放菊花台",
//    			"$播放一首京剧",
//    			"$我想听一个故事",
//    			"$播放海的女儿",
//    			"$打开北京交通广播",
//    			"$打开交通广播",
//    			"$我想听音乐广播"
    	};
        NLPCore test = new NLPCore();
        try {
        	for (String str : words) {
        		String s = test.ReturnMessage(str);
        		System.out.println(s);
			} 
        } catch (LoadModelException ex) {
            Logger.getLogger(NLPCore.class.getName()).log(Level.SEVERE, null, ex);
        }
    }
    
    public NLPCore()
    {  
    	dictPath = Config.getInstance().get("model_path", "C:/dict/");
    }
    
	private void init() {
		if (inited ){
			return;
		}
		
		inited = true;
		noResultList = new ArrayList<String>();
    	GetFileToList(noResultList,dictPath + "noresult.txt");
    	noResultIndex = (noResultIndex+1)%noResultList.size();
    	deviceType = new ArrayList<String>();
    	getSource = new GetSource();
    	getSource.LoadDeviceType(deviceType);
    	getSource.GetVerb2();
    	getSource.GetDevice2();
    	getSource.GetLast2();
    	getSource.GetGenre();
        getMusic = new SearchMusic();
        dbSearcher = new DBSearch();
        ctp = new ChineseToPinyin();
//        control = new ControlService();
       
//        ControlDelegate = control.getControlPort();
        this.mq = new messageQueue();
        try {   
            factory = CNFactory.getInstance(dictPath + "models");
        } catch (LoadModelException ex) {
            Logger.getLogger(NLPCore.class.getName()).log(Level.SEVERE, null, ex);
        }
        
        try {
            CNFactory.loadDict(dictPath + "mydict2.txt");
            CNFactory.loadDict(dictPath + "genredict.txt");
            CNFactory.loadDict(dictPath + "verbdict2.txt");
            CNFactory.loadDict(dictPath + "lastdict2.txt");
            CNFactory.loadDict(dictPath + "devicedict2.txt");
            //CNFactory.loadDict(curDir + "mysongdict.txt");
        } catch (LoadModelException ex) {
            Logger.getLogger(NLPCore.class.getName()).log(Level.SEVERE, null, ex);
        }
	}
    public String ReturnMessage(String sentence) throws LoadModelException
	{        
    	init();
       if(sentence.charAt(0) == 'C')
       {
    	   if(sentence.charAt(1) == '@')
    		   return ControlDelegate.getBing(sentence.substring(2));
    	   else if(sentence.charAt(1) == '#')
    		   return ControlDelegate.getSobot(sentence.substring(2));
    	   else if(sentence.charAt(1) == '$')
    		   return ControlDelegate.getTuLing(sentence.substring(2));
       }
       String initWords = sentence.substring(1);
       //initWords = ctp.FindAndResolveVerb(initWords);
	   String result = factory.tag2String(initWords);
	   System.out.println(result);
	   Hashtable<String, String> myWords = new Hashtable<String, String>();
           Hashtable<String, Integer> verbIndex = new Hashtable<String, Integer>();
	   String[] words = result.split(" ");
            for (int i = words.length - 1; i >= 0; i--)//从后往前写保证离主语最近的同类动词被处理
            {
                String word = words[i];
                String[] pos = word.split("/");
                myWords.put(pos[1], pos[0]);
                verbIndex.put(pos[1], i);
            }
	   String  remindMessage= "";
	   switch (JudgeDomain(myWords, verbIndex, deviceType))
	   {
	   case 1:remindMessage = HomeDomain(myWords, initWords);break;
	   case 2:remindMessage = MusicDomain(myWords,initWords);break;
	   case 3:remindMessage = LookupDomain(myWords,initWords);break;
	   case 4:remindMessage = robotDomain(myWords,initWords,sentence);break;
	   case 5:remindMessage = weatherDomain(myWords,initWords,sentence);break;
	   }
	   return remindMessage;	    
	}
    
    private String weatherDomain(Hashtable<String, String> myWords,
			String initWords, String sentence) {
		String location = myWords.get("地名");
		if (location == null || location.length() == 0) {
			location = "北京";
		}
		String time = myWords.get("时间短语");
		Date from = new Date();
		Date end = new Date();
		
		Pattern pto = Pattern.compile("([0-9]{1,2})月([0-9]{1,2})[号|日][至|到]([0-9]{1,2})月([0-9]{1,2})[号|日]");
		Matcher m = pto.matcher(initWords);
		if (m.find()) {
			String month = m.group(1);
			String day = m.group(2);
			String monthTo = m.group(3);
			String dayTo = m.group(4);
			
			Calendar calendar = Calendar.getInstance();
			calendar.set(Calendar.MONTH, Integer.parseInt(month) - 1);
			calendar.set(Calendar.DAY_OF_MONTH, Integer.parseInt(day));
			
			from = calendar.getTime();
			
			calendar = Calendar.getInstance();
			calendar.set(Calendar.MONTH, Integer.parseInt(monthTo) - 1);
			calendar.set(Calendar.DAY_OF_MONTH, Integer.parseInt(dayTo));
			end = calendar.getTime();
		} else {
			Pattern p = Pattern.compile("([0-9]{1,2})月([0-9]{1,2})[号|日]");
			Matcher m2 = p.matcher(initWords);
			
			if (m2.find()) {
				String month = m2.group(1);
			String day = m2.group(2);
				
				Calendar calendar = Calendar.getInstance();
				calendar.set(Calendar.MONTH, Integer.parseInt(month) - 1);
				calendar.set(Calendar.DAY_OF_MONTH, Integer.parseInt(day));
				
				from = calendar.getTime();
				end = from;
			} else {
				if (time == null || time.length() == 0) {
					time = myWords.get("时间模糊短语");
					if (time == null || time.length() == 0) {
						//time = "今天";
					}else {
						end.setTime(from.getTime() + 48*3600000);
					}
				} else {
					if (time.equals("今天")) {
						
					} else if (time.equals("明天")){
						from.setTime(from.getTime() + 24*3600000);
						end.setTime(from.getTime());
					} else if (time.equals("后天")){
						from.setTime(from.getTime() + 48*3600000);
						end.setTime(from.getTime());
					}
				}
			}
		}
		
		return WeatherService.getWeatherByCityName(location, from, end);
	}

	public int JudgeDomain(Hashtable<String, String> myWords, Hashtable<String, Integer> verbIndex, ArrayList<String> homeObject)
    {   
        String firstVerb = "";
        int maxVerbNum = 1000;
        int firstVerbIndex = maxVerbNum;
        String mediaVerb = myWords.get("媒体动词");
        if(mediaVerb != null)
        {
            int index = verbIndex.get("媒体动词");
            if(index < firstVerbIndex)
            {
                firstVerbIndex = index;
                firstVerb = mediaVerb;
            }
        }
        String homeVerb = myWords.get("家居动词");
        if(homeVerb != null)
        {
            int index = verbIndex.get("家居动词");
            if(index < firstVerbIndex)
            {
                firstVerbIndex = index;
                firstVerb = homeVerb;
            }
        }
        String searchVerb = myWords.get("查询动词");
        if(searchVerb != null)
        {
            int index = verbIndex.get("查询动词");
            if(index < firstVerbIndex)
            {
                firstVerbIndex = index;
                firstVerb = searchVerb;
            }
        }
        String fuzzyVerb = myWords.get("混合动词");
        if(fuzzyVerb != null)
        {
            int index = verbIndex.get("混合动词");
            if(index < firstVerbIndex)
            {
                firstVerbIndex = index;
                firstVerb = fuzzyVerb;
            }
        }
        
        if (verbIndex.containsKey("天气交通后缀词") && (verbIndex.containsKey("地名") || verbIndex.containsKey("时间短语"))) {
        	return 5;
        }
        
        
        if(firstVerbIndex == maxVerbNum || firstVerb.equals(""))
        {
            return 4;
        }
        
        //家居场景
        if(homeVerb != null && firstVerb.equals(homeVerb))
        {
            boolean flag = false;
            for(int i = 0; i < homeObject.size(); i++)
            {              
                String s = myWords.get(homeObject.get(i));
                if(s != null)
                {
                    flag = true;
                    break;
                }
            }
            if(flag)
                return 1;
            else
                return 4;          
        }
        
        if(mediaVerb != null && firstVerb.equals(mediaVerb))
        {
            return 2;
        }
        
        if(searchVerb != null && firstVerb.equals(searchVerb))
        {
            return 3;
        }
        
        if(fuzzyVerb != null && firstVerb.equals(fuzzyVerb))
        {
            boolean flag = false;
            for(int i = 0; i < homeObject.size(); i++)
            {              
                String s = myWords.get(homeObject.get(i));
                if(s != null)
                {
                    flag = true;
                    break;
                }
            }
            if(flag)
                return 1;
            else
                return 2;       
        }
        return 4;
    }
    private String robotDomain(Hashtable<String, String> myWords, String initWords, String sentence) {
		System.out.println("传给小冰");
		String message = "";
		if (sentence.charAt(0) == '@')
			message = ControlDelegate.getBing(initWords);
		else if (sentence.charAt(0) == '#')
			message = ControlDelegate.getSobot(initWords);
		else if (sentence.charAt(0) == '$')
			message = ControlDelegate.getTuLing(initWords);
		System.out.println(message);
		return message;

	}
	private String LookupDomain(Hashtable<String, String> myWords, String initWords) {
	    String dictVerb = myWords.get("查询动词");
		String newsAndWeather = myWords.get("天气交通后缀词");
		String message = "";
        if(dictVerb != null || newsAndWeather != null){              	
            String dictLast = myWords.get("词典后缀词"); //如果含有了词典后缀词，又没有搜到，则一定返回“主人我没有找到这个词”              
            if(dictLast!=null && !dictLast.equals("")){
                String noLast = initWords.substring(initWords.indexOf(dictVerb) + dictVerb.length(), initWords.indexOf(dictLast));                       
                String searchNoLast = dbSearcher.SearchOneWordInDict(noLast);
                if(searchNoLast != null){
                    System.out.println(noLast + "$" + searchNoLast);
                    return noLast + "," +searchNoLast;
                }
                else
                	return "主人我没有找到这个词";                       
            } 
          //如果不含有词典后缀词，又没有天气类后缀词，则会进行全词查询，如果查不到，跳出 if查询其他情况  
            if(newsAndWeather == null){
                String bin= initWords.substring(initWords.indexOf(dictVerb) + dictVerb.length());                                
                String searchNoLast = dbSearcher.SearchOneWordInDict(bin);
                if(searchNoLast != null){
                    System.out.println(bin + "$" + searchNoLast);
                    return bin + "," +searchNoLast;
                }
            }
            
            String location = myWords.get("ROOM");
        	if (location != null) {
        		String air = myWords.get("AIR");
        		if (air != null) {
        			String status = DeviceControlProxy.queryDeviceStatus(air + "@" + location);
					if (status != null) {
						return status;
					}
        		}
        	}
        	
            message = ControlDelegate.getTuLing(initWords);
            System.out.println(message);  
            return message;
        }
		return null;
	}

	private String MusicDomain(Hashtable<String, String> myWords,
 String initWords) {
		String message = "";
		String verb = myWords.get("媒体动词");
		if (verb == null)
			verb = myWords.get("混合动词");
		String genreVerb = myWords.get("音乐风格");
		String verbCommand = dbSearcher.searchCommand2(verb);
		if (verbCommand == null) {
			String noResult = noResultList.get(noResultIndex);
			noResultIndex = (noResultIndex + 1) % noResultList.size();
			System.out.println(noResult);
			return noResult;
		}
		if (verbCommand.equals("stop")) {// 停止播放
			message = "STOP" + "$" + "X" + "$" + "X" + "$" + "X";
			mq.Sender(message);
			System.out.println(message);
			return "SUCCESS";
		}

		if (genreVerb != null && verb != null) {// 播放某种风格歌曲，如播放流行音乐
			// 修改by v-feiche
			String songName = initWords.substring(initWords.indexOf(genreVerb) + genreVerb.length());
			String genreSearchResult = dbSearcher.SearchGenre(genreVerb, songName);
			if (genreSearchResult.length() > 1) {
				System.out.println(verbCommand.toUpperCase() + "$" + genreSearchResult);
				mq.Sender(verbCommand.toUpperCase() + "$" + genreSearchResult);
				return "SUCCESS";
			}
		}
		if (verb != null)// 媒体场景
		{
			String bin = null;
			String noLastResult = "";
			String noLastStoryResult = "";
			String noLastRandomResult = "";
			String noLastRadioResult = "";
			String noLastResultWithSinger = "";
			String resultWithSinger = "";
			String randomMediaResult = "";
			String afterOneTimeSearch = "";
			String afterOneTimeSearch1 = "";
			bin = initWords.substring(initWords.indexOf(verb) + verb.length());
			System.out.println(bin);
			if (bin.equals("故事") || bin.equals("童话")) {
				randomMediaResult = dbSearcher.RandomSearchOneWordWithCategory("story");
				if (randomMediaResult.length() > 1) {
					System.out.println(verbCommand.toUpperCase() + "$" + randomMediaResult);
					mq.Sender(verbCommand.toUpperCase() + "$" + randomMediaResult);
					return "SUCCESS";
				}
			}
			if (bin.equals("广播") || bin.equals("电台") || bin.equals("调频")) {
				randomMediaResult = dbSearcher.RandomSearchOneWordWithCategory("radio");
				if (randomMediaResult.length() > 1) {
					System.out.println(verbCommand.toUpperCase() + "$" + randomMediaResult);
					mq.Sender(verbCommand.toUpperCase() + "$" + randomMediaResult);
					return "SUCCESS";
				}
			}
			if (bin.equals("歌") || bin.equals("歌曲") || bin.equals("音乐")) {
				// randomMediaResult = RandomSearchOneWordWithCategory("music");
				randomMediaResult = dbSearcher.RandomSearchMusicFromBaidu();
				if (randomMediaResult.length() > 1) {
					System.out.println(verbCommand.toUpperCase() + "$" + randomMediaResult);
					mq.Sender(verbCommand.toUpperCase() + "$" + randomMediaResult);
					return "SUCCESS";
				}
			}
			// 我想听白雪公主/浮夸/北京广播电台
			String searchTotalBinResult = dbSearcher.RandomSearchMusicFromBaidu(bin, "");
			System.out.println(searchTotalBinResult);
			if (searchTotalBinResult.length() > 1) {
				System.out.println(verbCommand.toUpperCase() + "$" + searchTotalBinResult);
				mq.Sender(verbCommand.toUpperCase() + "$" + searchTotalBinResult);
				return "SUCCESS";
			}

			String last = null;
			String storyLast = null;
			String randomLast = null;
			String radioLast = null;
			// 作者的字定语词
			String deZiDingYu = null;
			last = myWords.get("音乐后缀词");
			storyLast = myWords.get("故事后缀词");
			randomLast = myWords.get("随机后缀词");
			radioLast = myWords.get("广播后缀词");
			deZiDingYu = myWords.get("作者的字定语词");
			if (!RealyLast(radioLast, initWords)) {
				radioLast = null;
			}
			// 处理音乐后缀词如播放十年这首歌
			if (last != null && !last.equals("")) {
				String noLast = initWords.substring(initWords.indexOf(verb) + verb.length(), initWords.indexOf(last));
				System.out.println(noLast);
				// noLastResult = SearchOneWord(noLast,"music");
				noLastResult = dbSearcher.RandomSearchMusicFromBaidu(noLast, "");
			}
			if (noLastResult.length() > 1) {
				System.out.println(verbCommand.toUpperCase() + "$" + noLastResult);
				mq.Sender(verbCommand.toUpperCase() + "$" + noLastResult);
				return "SUCCESS";
			}
			// 处理音乐后缀词如播放陈奕迅的十年这首歌
			String afterVerbSinger1 = "";
			String afterVerbSong1 = "";
			if (last != null && !last.equals("")) {
				String noLast = initWords.substring(initWords.indexOf(verb) + verb.length(), initWords.indexOf(last));
				System.out.println(noLast);
				String afterVerb = noLast;
				if (deZiDingYu != null)
					afterVerb = afterVerb.replace(deZiDingYu, "的");
				int deIndex = afterVerb.indexOf("的");
				afterVerbSinger1 = afterVerb.substring(0, deIndex);
				afterVerbSong1 = afterVerb.substring(deIndex + 1);
				System.out.println(afterVerbSinger1 + "$" + afterVerbSong1);
				// noLastResultWithSinger =
				// SearchOneWordSingerAndName(afterVerbSinger,afterVerbSong);
				noLastResultWithSinger = dbSearcher.RandomSearchMusicFromBaidu(afterVerbSong1, afterVerbSinger1);
				System.out.println(noLastResultWithSinger);
				if (noLastResultWithSinger.length() > 1) {
					System.out.println(verbCommand.toUpperCase() + "$" + noLastResultWithSinger);
					mq.Sender(verbCommand.toUpperCase() + "$" + noLastResultWithSinger);
					return "SUCCESS";
				} else {// 如果没有搜索到陈奕迅的十年，则任选一首十年播放
					afterOneTimeSearch = dbSearcher.RandomSearchMusicFromBaidu(afterVerbSong1, "");
					if (afterOneTimeSearch.length() > 1) {
						System.out.println(afterOneTimeSearch);
						mq.Sender(verbCommand.toUpperCase() + "$" + afterOneTimeSearch);
						String afterOneTimeSearchList[] = afterOneTimeSearch.split("\\$");
						if (afterOneTimeSearchList != null) {
							if (afterOneTimeSearchList.length == 3) {
								System.out.println("主人，我没有帮您找到" + afterVerbSinger1 + "的" + afterVerbSong1 + ",我为您播放了" + afterOneTimeSearchList[2]);
								return "主人，我没有帮您找到" + afterVerbSinger1 + "的" + afterVerbSong1 + ",我为您播放了" + afterOneTimeSearchList[2];
							} else {
								System.out.println("主人，我没有帮您找到" + afterVerbSinger1 + "的" + afterVerbSong1 + ",我为您播放了另一首十年");
								return "主人，我没有帮您找到" + afterVerbSinger1 + "的" + afterVerbSong1 + ",我为您播放了另一首十年";
							}
						}
					}
					String singerValid = dbSearcher.RandomSearchMusicFromBaidu("", afterVerbSinger1);
					if (singerValid.length() > 1) {
						System.out.println("主人，我没有帮您找到" + afterVerbSinger1 + "的" + afterVerbSong1);
						return "主人，我没有帮您找到" + afterVerbSinger1 + "的" + afterVerbSong1;
					}
				}
			}

			// 处理故事后缀词 如播放白雪公主这个故事
			if (storyLast != null && !storyLast.equals("")) {
				String noLast = initWords.substring(initWords.indexOf(verb) + verb.length(), initWords.indexOf(storyLast));
				System.out.println(noLast);
				noLastStoryResult = dbSearcher.SearchOneWord(noLast, "story");
			}
			if (noLastStoryResult.length() > 1) {
				System.out.println(verbCommand.toUpperCase() + "$" + noLastStoryResult);
				mq.Sender(verbCommand.toUpperCase() + "$" + noLastStoryResult);
				return "SUCCESS";
			}
			// 处理随机歌曲后缀词如播放周杰伦的歌
			if (randomLast != null && !randomLast.equals("")) {
				String noLast = initWords.substring(initWords.indexOf(verb) + verb.length(), initWords.indexOf(randomLast));
				System.out.println(noLast);
				// noLastRandomResult =
				// RandomSearchOneWordWithSinger(noLast,"music","Metadata.Singer");
				noLastRandomResult = dbSearcher.RandomSearchMusicFromBaidu("", noLast);
				System.out.println(noLastRandomResult);
			}
			if (noLastRandomResult.length() > 1) {
				System.out.println(verbCommand.toUpperCase() + "$" + noLastRandomResult);
				mq.Sender(verbCommand.toUpperCase() + "$" + noLastRandomResult);
				return "SUCCESS";
			}
			// 处理广播后缀词
			if (radioLast != null && !radioLast.equals("")) {
				String noLast = initWords.substring(initWords.indexOf(verb) + verb.length(), initWords.indexOf(radioLast));
				noLastRadioResult = dbSearcher.SearchRadioCategory(noLast);
				// noLastRadioResult =dbSearcher.SearchOneWord(noLast,"radio");
				System.out.println(noLastRadioResult);
			}
			if (noLastRadioResult.length() > 1) {
				System.out.println(verbCommand.toUpperCase() + "$" + noLastRadioResult);
				mq.Sender(verbCommand.toUpperCase() + "$" + noLastRadioResult);
				return "SUCCESS";
			}
			// 处理陈奕迅的十年这种情况
			String afterVerbSinger2 = "";
			String afterVerbSong2 = "";
			String afterVerb = initWords.substring(initWords.indexOf(verb) + verb.length());
			if (deZiDingYu != null)
				afterVerb = afterVerb.replace(deZiDingYu, "的");
			int deIndex = afterVerb.indexOf("的");
			if (deIndex != -1) {
				afterVerbSinger2 = afterVerb.substring(0, deIndex);
				afterVerbSong2 = afterVerb.substring(deIndex + 1);
				System.out.println(afterVerbSinger2 + "：" + afterVerbSong2);
				// resultWithSinger =
				// SearchOneWordSingerAndName(afterVerbSinger,afterVerbSong);
				resultWithSinger = dbSearcher.RandomSearchMusicFromBaidu(afterVerbSong2, afterVerbSinger2);
			}
			if (resultWithSinger.length() > 1) {
				System.out.println(verbCommand.toUpperCase() + "$" + resultWithSinger);
				mq.Sender(verbCommand.toUpperCase() + "$" + resultWithSinger);
				return "SUCCESS";
			} else {// 如果没有搜索到陈奕迅的十年，则任选一首十年播放
				afterOneTimeSearch1 = dbSearcher.RandomSearchMusicFromBaidu(afterVerbSong2, "");
				if (afterOneTimeSearch1.length() > 1) {
					System.out.println(afterOneTimeSearch1);
					mq.Sender(verbCommand.toUpperCase() + "$" + afterOneTimeSearch1);
					String afterOneTimeSearchList[] = afterOneTimeSearch1.split("\\$");
					if (afterOneTimeSearchList != null) {
						if (afterOneTimeSearchList.length == 3) {
							System.out.println("主人，我没有帮您找到" + afterVerbSinger2 + "的" + afterVerbSong2 + ",我为您播放了" + afterOneTimeSearchList[2]);
							return "主人，我没有帮您找到" + afterVerbSinger2 + "的" + afterVerbSong2 + ",我为您播放了" + afterOneTimeSearchList[2];
						} else {
							System.out.println("主人，我没有帮您找到" + afterVerbSinger2 + "的" + afterVerbSong2 + ",我为您播放了另一首十年");
							return "主人，我没有帮您找到" + afterVerbSinger2 + "的" + afterVerbSong2 + ",我为您播放了另一首十年";
						}
					}

				}
				String singerValid = dbSearcher.RandomSearchMusicFromBaidu("", afterVerbSinger2);
				if (singerValid.length() > 1) {
					System.out.println("主人，我没有帮您找到" + afterVerbSinger2 + "的" + afterVerbSong2);
					return "主人，我没有帮您找到" + afterVerbSinger2 + "的" + afterVerbSong2;
				} else if (afterOneTimeSearch1.equals("$") || afterOneTimeSearch1.equals("")) { // 随机选择一个返回语句
					String noResult = noResultList.get(noResultIndex);
					noResultIndex = (noResultIndex + 1) % noResultList.size();
					System.out.println(noResult);
					return noResult;
				}
			}
		}
		return null;
	}

	private String HomeDomain(Hashtable<String, String> myWords, String initWords) {
		String homeVerb = myWords.get("家居动词");
		if(homeVerb == null)
			homeVerb = myWords.get("混合动词");
		String message = "";
		if (homeVerb != null) {// 家居场景Z
			String homeVerbCommand = dbSearcher.searchCommand2(homeVerb);
			if(homeVerbCommand == null)
				return "对不起，我没有理解您的意思";
			
			String temValue = myWords.get("温度词");
			String target = myWords.get("LIGHT");
			String targetClass = "LIGHT";
			if (target == null || target.equals("")) {
				target = myWords.get("CURTAIN");
				targetClass = "CURTAIN";
			}
			if (target == null || target.equals("")) {
				target = myWords.get("AIR");
				targetClass = "AIR";
			}
			if (target == null || target.equals("")) {
				target = myWords.get("airpurifier");
				targetClass = "airpuriier";
			}
			if (target != null) {
				String room = myWords.get("ROOM");
				if (room == null)
					room = "X";
				if ((homeVerbCommand.equals("down")
						|| homeVerbCommand.equals("up") || homeVerbCommand
							.equals("change") || homeVerbCommand.equals("start") || homeVerbCommand.equals("stop"))
						) {
					if (temValue != null
							&& !temValue.equals("")) {
						message = homeVerbCommand.toUpperCase() + "$" + targetClass
								+ "$" + room.toUpperCase() + "$" + temValue + "$"
								+ target.toUpperCase();
						mq.Sender(message);
						System.out.println(message);
					} else if (targetClass.equals("airpuriier")){
						DeviceControlProxy.control(targetClass, homeVerbCommand, "");
						return "SUCCESS";
					} else {
						message = homeVerbCommand.toUpperCase() + "$" + targetClass
								+ "$" + room.toUpperCase() + "$" + target.toUpperCase();
						mq.Sender(message);
						System.out.println(message);
						return "SUCCESS";
					}
				}
			}
		}
		return null;
	}
	private int GetDomain() {
		// TODO Auto-generated method stub
		return 1;
	}
	public boolean RealyLast(String last, String initWords) {
    	if(last != null){
			int index = initWords.indexOf(last);
			if(initWords.length() == (index + last.length()))
				return true;
			return false;
    	}
    	return false;
	}
	public  void GetFileToList(ArrayList<String> oneList, String path) {
 		// TODO Auto-generated method stub
     	File file = new File(path);
         BufferedReader reader = null;
         try {
             //System.out.println("以行为单位读取文件内容，一次读一整行：");
             reader = new BufferedReader(new FileReader(file));
             String tempString = null;
             int line = 0;
             // 一次读入一行，直到读入null为文件结束
             while ((tempString = reader.readLine()) != null) {
                 // 显示行号
                 //System.out.println("line " + line + ": " + tempString);
             	oneList.add(tempString);
                // line++;
             }
             reader.close();
         } catch (IOException e) {
             e.printStackTrace();
         } finally {
             if (reader != null) {
                 try {
                     reader.close();
                 } catch (IOException e1) {
                 }
             }
         }
 	}
}
