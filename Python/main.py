import requests
import re
import json
 
def re_domain(url):
   pattern_domain = r"https?://([^/]+)"
   match = re.search(pattern_domain, url)
   return match.group(1) if match else None
 
def Get_lzy(url, password):
   headers = {
       "User-Agent": "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/121.0.0.0 Safari/537.36 Edg/121.0.0.0"
   }
 
   response = requests.get(url, headers=headers)
   url_match = re.search(r"url\s*:\s*'(/ajaxm\.php\?file=\d+)'", response.text).group(1)
   skdklds_match = re.search(r"var\s+skdklds\s*=\s*'([^']*)';", response.text).group(1)
 
   data = {
       'action': 'downprocess',
       'sign': skdklds_match,
       'p': password,
   }
   headers.update({
       "Referer": url
   })
   domain = re_domain(url)
   response2 = requests.post(f"https://{domain}{url_match}", headers=headers, data=data)
   data = json.loads(response2.text)
   full_url = data['dom'] + "/file/" + data['url']
 
   headers = {
       "accept": "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7",
       "accept-language": "zh-CN,zh;q=0.9,en;q=0.8,en-GB;q=0.7,en-US;q=0.6",
       "sec-ch-ua": "\"Chromium\";v=\"122\", \"Not(A:Brand\";v=\"24\", \"Microsoft Edge\";v=\"122\"",
       "sec-ch-ua-mobile": "?0",
       "sec-ch-ua-platform": "\"Windows\"",
       "sec-fetch-dest": "document",
       "sec-fetch-mode": "navigate",
       "sec-fetch-site": "none",
       "sec-fetch-user": "?1",
       "upgrade-insecure-requests": "1",
       "cookie": "down_ip=1"
   }
   response3 = requests.get(full_url, headers=headers, allow_redirects=False)
   redirect_url = response3.headers['Location']
   return redirect_url
 
print(get_lzy("地址", "密码"))