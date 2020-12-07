using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace AsLink
{
    public partial class WebHelper
    {
        public static async Task<string> GetWebStr(string url, TimeSpan? expiryPeriod = null)
        {
            var sw = Stopwatch.StartNew();
            var now = DateTime.Now;

            try
            {
                if (expiryPeriod != null)
                {
                    var cached = await WebCacheHelper.TryGetFromCache(url);
                    if (cached != null && cached.Length > 100) //todo: && did not ask for the same twice in 1 minute (Jun2016)
                        return cached;
                }

#if OFFLINE
      await Task.Delay(33);
      if (url.Contains("agencyList")) return _al;       // url == "https://webservices.nextbus.com/service/publicXMLFeed?command=agencyList") return _al;
      if (url.Contains("routeList")) return _rl;        // url == "https://webservices.nextbus.com/service/publicXMLFeed?command=routeList&a=ttc" || 
      if (url.Contains("routeConfig")) return _rc;      // url == "https://webservices.nextbus.com/service/publicXMLFeed?command=routeConfig&a=ttc&r=125" || 
      if (url.Contains("predictions")) return _pr;      // url == "https://webservices.nextbus.com/service/publicXMLFeed?command=predictions&a=actransit&r=5&s=4181&useShortTitles=false" || 
      if (url.Contains("vehicleLocations")) return _lo; // url == "https://webservices.nextbus.com/service/publicXMLFeed?command=vehicleLocations&a=actransit&r=5&t=0" || 
#endif
                var httpClientHandler = new HttpClientHandler { Proxy = WebRequest.DefaultWebProxy, UseProxy = true };
                httpClientHandler.Proxy.Credentials = CredentialCache.DefaultNetworkCredentials; //TU: 2/2 //note: is not required for getiing pics and files - html only (3-jun-2010).

                using (var client = new HttpClient(httpClientHandler))
                {
                    try
                    {
                        //ient.DefaultRequestHeaders.Add("Expires", "99999");                           // crashes
                        //ient.DefaultRequestHeaders.Add("Cache-Control", "max-age=604800, public");    //no changes   (604800 == 7 days)
                        client.DefaultRequestHeaders.Add("Cache-Control", "no-cache");                  //tu: avoid caching!!!

                        var str = await client.GetStringAsync(url);

                        if (expiryPeriod != null)
                            await WebCacheHelper.PutToCache(url, str, now + expiryPeriod.Value);

                        return str;
                    }
                    catch (HttpRequestException ex) { return string.Format("\n HttpRequestException: {0} \n {1}", now, ex); } // ignore disconnected case.
                    catch (Exception ex) { return string.Format("\n Exception: {0} \n {1}", now, ex); }
                }
            }
            catch (Exception ex) { return string.Format("\n Exception: {0} \n {1}", now, ex); }
            finally { /*Debug.WriteLine("°");*/ }//==> Caching test: {0,8:N0} ms  for '{1}' (usually 100ms) for url   {2}.", sw.ElapsedMilliseconds, expiryPeriod != null ? "tryCached" : "nonCached", url); }
        }

        #region DevOp_OffLine
        const string
    _pr = @"<?xml version='1.0' encoding='utf-8' ?> 
<body copyright='All data copyright Toronto Transit Commission 2015.'>
<predictions agencyTitle='Toronto Transit Commission' routeTitle='60-Steeles West' routeTag='60' stopTitle='Steeles Ave West At Walkway To Cathy Jean Cres' stopTag='9724'>
  <direction title='East - 60 Steeles East towards Finch Station'>
  <prediction epochTime='1451138532488' seconds='352' minutes='5' isDeparture='false' affectedByLayover='true' branch='60' dirTag='60_0_60B' vehicle='8469' block='60_1_10' tripTag='29890875' />
  <prediction epochTime='1451139312488' seconds='1132' minutes='18' isDeparture='false' affectedByLayover='true' branch='60' dirTag='60_0_60B' vehicle='7972' block='60_2_20' tripTag='29890794' />
  <prediction epochTime='1451140872488' seconds='2692' minutes='44' isDeparture='false' affectedByLayover='true' branch='60' dirTag='60_0_60B' vehicle='8464' block='60_6_60' tripTag='29890757' />
  <prediction epochTime='1451141652488' seconds='3472' minutes='57' isDeparture='false' affectedByLayover='true' branch='60' dirTag='60_0_60B' vehicle='8406' block='60_4_40' tripTag='29890759' />
  </direction>
</predictions>
</body>
",
    _lo = @"<?xml version='1.0' encoding='utf-8' ?> 
<body copyright='All data copyright Toronto Transit Commission 2015.'>
<vehicle id='8469' routeTag='60' dirTag='60_1_60B' lat='43.756485' lon='-79.608116' secsSinceReport='-5' predictable='true' heading='85'/>
<vehicle id='7972' routeTag='60' dirTag='60_1_60B' lat='43.765968' lon='-79.567886' secsSinceReport='-4' predictable='true' heading='253'/>
<vehicle id='8464' routeTag='60' dirTag='60_1_60B' lat='43.787483' lon='-79.417282' secsSinceReport='-8' predictable='true' heading='350'/>
<vehicle id='1258' routeTag='60' dirTag='60_1_60C' lat='43.789982' lon='-79.457397' secsSinceReport='-5' predictable='true' heading='254'/>
<vehicle id='8456' routeTag='60' dirTag='60_0_60B' lat='43.7701' lon='-79.5468829' secsSinceReport='-4' predictable='true' heading='75'/>
<vehicle id='1309' routeTag='60' dirTag='60_0_60C' lat='43.78455' lon='-79.481232' secsSinceReport='0' predictable='true' heading='70'/>
<vehicle id='8406' routeTag='60' dirTag='60_0_60C' lat='43.785465' lon='-79.417015' secsSinceReport='-2' predictable='true' heading='170'/>
<vehicle id='8443' routeTag='60' dirTag='60_1_60C' lat='43.77895' lon='-79.418869' secsSinceReport='-9' predictable='true' heading='216'/>
<vehicle id='7974' routeTag='60' dirTag='60_0_60B' lat='43.793068' lon='-79.442566' secsSinceReport='-1' predictable='true' heading='73'/>
<lastTime time='1451138233797'/>
</body>
",
    _rc = @"<?xml version='1.0' encoding='utf-8' ?> 
<body copyright='All data copyright Toronto Transit Commission 2015.'>
<route tag='125' title='125-Drewry' color='ff0000' oppositeColor='ffffff' latMin='43.7793799' latMax='43.7870699' lonMin='-79.45176' lonMax='-79.41519'>
<stop tag='1842' title='Antibes Dr (South) At Torresdale Ave East Side' lat='43.7793799' lon='-79.45176' stopId='9281'/>
<stop tag='4181' title='Antibes Dr (South) At Candle Liteway (East)' lat='43.77965' lon='-79.4491299' stopId='9282'/>
<stop tag='1192' title='Opposite 100 Antibes Dr' lat='43.7796111' lon='-79.4469939' stopId='9283'/>
<stop tag='9316' title='Antibes Dr At Bathurst St' lat='43.78095' lon='-79.44512' stopId='9284'/>
<stop tag='7410' title='Drewry Ave At Marthon Cres (East)' lat='43.7818199' lon='-79.44109' stopId='9285'/>
<stop tag='8590' title='Drewry Ave At Lister Dr' lat='43.78256' lon='-79.4377199' stopId='9286'/>
<stop tag='8026' title='Drewry Ave At Grantbrook St' lat='43.7834' lon='-79.4338999' stopId='9287'/>
<stop tag='289' title='Drewry Ave At Gardenia Crt' lat='43.7838899' lon='-79.43172' stopId='9288'/>
<stop tag='4617' title='Drewry Ave At Norwin St' lat='43.78464' lon='-79.4283599' stopId='9289'/>
<stop tag='1603' title='Drewry Ave At Hilda Ave' lat='43.78555' lon='-79.4242299' stopId='9290'/>
<stop tag='1279' title='Drewry Ave At Fairchild Ave' lat='43.7861' lon='-79.4218299' stopId='9291'/>
<stop tag='2079' title='43 Drewry Ave' lat='43.78661' lon='-79.4194899' stopId='9292'/>
<stop tag='992' title='Drewry Ave At Yonge St' lat='43.7869' lon='-79.4180699' stopId='9293'/>
<stop tag='900' title='5800 Yonge St' lat='43.7842099' lon='-79.41677' stopId='9013'/>
<stop tag='14212_ar' title='Finch Station' lat='43.7814299' lon='-79.41519'/>
<stop tag='14212' title='Finch Station' lat='43.7814299' lon='-79.41519' stopId='14692'/>
<stop tag='1805' title='Yonge St At Bishop Ave North Side (Finch Station)' lat='43.7818799' lon='-79.41593' stopId='3212'/>
<stop tag='15269' title='5799 Yonge St' lat='43.7853399' lon='-79.4167999' stopId='15447'/>
<stop tag='7676' title='Drewry Ave At Yonge St West Side' lat='43.7870699' lon='-79.41778' stopId='9269'/>
<stop tag='2453' title='30 Drewry Ave' lat='43.7866799' lon='-79.41949' stopId='9270'/>
<stop tag='8322' title='Drewry Ave At Fairchild Ave' lat='43.7862399' lon='-79.42154' stopId='9271'/>
<stop tag='8114' title='Drewry Ave At Hilda Ave' lat='43.78574' lon='-79.4239499' stopId='9272'/>
<stop tag='10316' title='Drewry Ave At Norwin St' lat='43.7848699' lon='-79.42772' stopId='9273'/>
<stop tag='4860' title='Drewry Ave At Cactus Ave' lat='43.78408' lon='-79.4311999' stopId='9274'/>
<stop tag='290' title='Drewry Ave At Grantbrook St' lat='43.78357' lon='-79.4335999' stopId='9275'/>
<stop tag='8515' title='Drewry Ave At Lister Dr' lat='43.7827799' lon='-79.4373' stopId='9276'/>
<stop tag='8076' title='Drewry Ave At Marathon Cres (East)' lat='43.7820599' lon='-79.4404' stopId='9277'/>
<stop tag='6249' title='Drewry Ave At Bathurst St' lat='43.7811399' lon='-79.44453' stopId='9278'/>
<stop tag='2137' title='Antibes Dr (North) At Plum Treeway (South) North Side' lat='43.7818499' lon='-79.44654' stopId='9279'/>
<stop tag='7025' title='Antibes Dr (North) At Torresdale Ave' lat='43.782' lon='-79.4498799' stopId='9280'/>
<stop tag='1842_ar' title='Antibes Dr (South) At Torresdale Ave East Side' lat='43.7793799' lon='-79.45176'/>
<direction tag='125_1_125' title='West - 125 Drewry towards Bathurst (Torresdale)' name='West' useForUI='true' branch='125'>
  <stop tag='14212' />
  <stop tag='1805' />
  <stop tag='15269' />
  <stop tag='7676' />
  <stop tag='2453' />
  <stop tag='8322' />
  <stop tag='8114' />
  <stop tag='10316' />
  <stop tag='4860' />
  <stop tag='290' />
  <stop tag='8515' />
  <stop tag='8076' />
  <stop tag='6249' />
  <stop tag='2137' />
  <stop tag='7025' />
  <stop tag='1842_ar' />
</direction>
<direction tag='125_0_125' title='East - 125 Drewry towards Finch Station' name='East' useForUI='true' branch='125'>
  <stop tag='1842' />
  <stop tag='4181' />
  <stop tag='1192' />
  <stop tag='9316' />
  <stop tag='7410' />
  <stop tag='8590' />
  <stop tag='8026' />
  <stop tag='289' />
  <stop tag='4617' />
  <stop tag='1603' />
  <stop tag='1279' />
  <stop tag='2079' />
  <stop tag='992' />
  <stop tag='900' />
  <stop tag='14212_ar' />
</direction>
<path>
<point lat='43.77938' lon='-79.4517599'/>
<point lat='43.77933' lon='-79.45141'/>
<point lat='43.77931' lon='-79.45102'/>
<point lat='43.77937' lon='-79.45061'/>
<point lat='43.77967' lon='-79.44922'/>
<point lat='43.7796499' lon='-79.44913'/>
<point lat='43.77973' lon='-79.44882'/>
<point lat='43.77975' lon='-79.44847'/>
<point lat='43.77973' lon='-79.44799'/>
<point lat='43.77969' lon='-79.44768'/>
<point lat='43.77962' lon='-79.44737'/>
<point lat='43.77961' lon='-79.447'/>
<point lat='43.77967' lon='-79.44668'/>
<point lat='43.77984' lon='-79.44635'/>
<point lat='43.7801199' lon='-79.44625'/>
<point lat='43.78074' lon='-79.44632'/>
<point lat='43.78079' lon='-79.44586'/>
<point lat='43.78099' lon='-79.44528'/>
<point lat='43.78095' lon='-79.44512'/>
<point lat='43.78103' lon='-79.44482'/>
<point lat='43.78182' lon='-79.44109'/>
</path>
<path>
<point lat='43.78421' lon='-79.41677'/>
<point lat='43.78163' lon='-79.4159899'/>
<point lat='43.7817099' lon='-79.41575'/>
<point lat='43.78178' lon='-79.41547'/>
<point lat='43.78183' lon='-79.41525'/>
<point lat='43.78175' lon='-79.41521'/>
<point lat='43.78175' lon='-79.41511'/>
<point lat='43.78168' lon='-79.41501'/>
<point lat='43.78035' lon='-79.41445'/>
<point lat='43.7802099' lon='-79.41446'/>
<point lat='43.78016' lon='-79.41459'/>
<point lat='43.78018' lon='-79.41475'/>
<point lat='43.78038' lon='-79.41485'/>
<point lat='43.78062' lon='-79.41497'/>
<point lat='43.78087' lon='-79.41508'/>
<point lat='43.78107' lon='-79.41516'/>
<point lat='43.78128' lon='-79.41524'/>
<point lat='43.78143' lon='-79.41519'/>
</path>
<path>
<point lat='43.78143' lon='-79.41519'/>
<point lat='43.78146' lon='-79.41531'/>
<point lat='43.78161' lon='-79.41533'/>
<point lat='43.78173' lon='-79.41531'/>
<point lat='43.78175' lon='-79.41521'/>
<point lat='43.78183' lon='-79.41525'/>
<point lat='43.78178' lon='-79.41547'/>
<point lat='43.7817099' lon='-79.41575'/>
<point lat='43.78163' lon='-79.4159899'/>
<point lat='43.78188' lon='-79.41593'/>
<point lat='43.78534' lon='-79.4168'/>
<point lat='43.7870599' lon='-79.41734'/>
<point lat='43.78707' lon='-79.4177799'/>
</path>
<path>
<point lat='43.78185' lon='-79.44654'/>
<point lat='43.78213' lon='-79.44673'/>
<point lat='43.7823' lon='-79.44699'/>
<point lat='43.7824' lon='-79.44727'/>
<point lat='43.78243' lon='-79.4476'/>
<point lat='43.7819999' lon='-79.4498799'/>
<point lat='43.78192' lon='-79.4498799'/>
<point lat='43.7819' lon='-79.45026'/>
<point lat='43.78177' lon='-79.45058'/>
<point lat='43.78079' lon='-79.45165'/>
<point lat='43.78057' lon='-79.45184'/>
<point lat='43.77959' lon='-79.45241'/>
<point lat='43.77938' lon='-79.4517599'/>
</path>
<path>
<point lat='43.78707' lon='-79.4177799'/>
<point lat='43.78702' lon='-79.4177799'/>
<point lat='43.7866799' lon='-79.41949'/>
<point lat='43.78624' lon='-79.4215399'/>
<point lat='43.7857399' lon='-79.42395'/>
<point lat='43.78487' lon='-79.4277199'/>
<point lat='43.78408' lon='-79.4312'/>
<point lat='43.78357' lon='-79.4336'/>
<point lat='43.78278' lon='-79.4372999'/>
<point lat='43.78206' lon='-79.4404'/>
<point lat='43.78114' lon='-79.44453'/>
<point lat='43.78103' lon='-79.44482'/>
<point lat='43.78109' lon='-79.44534'/>
<point lat='43.7812' lon='-79.44562'/>
<point lat='43.78121' lon='-79.44636'/>
<point lat='43.78144' lon='-79.44643'/>
<point lat='43.78185' lon='-79.44654'/>
</path>
<path>
<point lat='43.78182' lon='-79.44109'/>
<point lat='43.7825599' lon='-79.43772'/>
<point lat='43.7834' lon='-79.4339'/>
<point lat='43.78389' lon='-79.43172'/>
<point lat='43.78464' lon='-79.42836'/>
<point lat='43.78555' lon='-79.42423'/>
<point lat='43.7861' lon='-79.42183'/>
<point lat='43.78661' lon='-79.41949'/>
<point lat='43.7869' lon='-79.41807'/>
<point lat='43.78699' lon='-79.41795'/>
<point lat='43.78704' lon='-79.41761'/>
<point lat='43.78616' lon='-79.4171099'/>
<point lat='43.78421' lon='-79.41677'/>
</path>
<path>
<point lat='43.77938' lon='-79.4517599'/>
<point lat='43.77933' lon='-79.45141'/>
<point lat='43.77931' lon='-79.45102'/>
<point lat='43.77937' lon='-79.45061'/>
<point lat='43.77967' lon='-79.44922'/>
<point lat='43.7796499' lon='-79.44913'/>
<point lat='43.77973' lon='-79.44882'/>
<point lat='43.77975' lon='-79.44847'/>
<point lat='43.77973' lon='-79.44799'/>
<point lat='43.77969' lon='-79.44768'/>
<point lat='43.77962' lon='-79.44737'/>
<point lat='43.77961' lon='-79.447'/>
<point lat='43.77967' lon='-79.44668'/>
<point lat='43.77984' lon='-79.44635'/>
<point lat='43.7801199' lon='-79.44625'/>
<point lat='43.78074' lon='-79.44632'/>
<point lat='43.78079' lon='-79.44586'/>
<point lat='43.78099' lon='-79.44528'/>
<point lat='43.78095' lon='-79.44512'/>
</path>
</route>
</body>
",
    _rl = @"<?xml version='1.0' encoding='utf-8' ?> 
<body copyright='All data copyright Toronto Transit Commission 2015.'>
<route tag='1S' title='1S-Yonge Subway Shuttle'/>
<route tag='5' title='5-Avenue Rd'/>
<route tag='6' title='6-Bay'/>
<route tag='7' title='7-Bathurst'/>
<route tag='8' title='8-Broadview'/>
<route tag='9' title='9-Bellamy'/>
<route tag='10' title='10-Van Horne'/>
<route tag='11' title='11-Bayview'/>
<route tag='12' title='12-Kingston Rd'/>
<route tag='14' title='14-Glencairn'/>
<route tag='15' title='15-Evans'/>
<route tag='16' title='16-McCowan'/>
<route tag='17' title='17-Birchmount'/>
<route tag='20' title='20-Cliffside'/>
<route tag='21' title='21-Brimley'/>
<route tag='22' title='22-Coxwell'/>
<route tag='23' title='23-Dawes'/>
<route tag='24' title='24-Victoria Park'/>
<route tag='25' title='25-Don Mills'/>
<route tag='26' title='26-Dupont'/>
<route tag='28' title='28-Davisville'/>
<route tag='29' title='29-Dufferin'/>
<route tag='30' title='30-Lambton'/>
<route tag='31' title='31-Greenwood'/>
<route tag='32' title='32-Eglinton West'/>
<route tag='33' title='33-Forest Hill'/>
<route tag='34' title='34-Eglinton East'/>
<route tag='35' title='35-Jane'/>
<route tag='36' title='36-Finch West'/>
<route tag='37' title='37-Islington'/>
<route tag='38' title='38-Highland Creek'/>
<route tag='39' title='39-Finch East'/>
<route tag='40' title='40-Junction'/>
<route tag='41' title='41-Keele'/>
<route tag='42' title='42-Cummer'/>
<route tag='43' title='43-Kennedy'/>
<route tag='44' title='44-Kipling South'/>
<route tag='45' title='45-Kipling'/>
<route tag='46' title='46-Martin Grove'/>
<route tag='47' title='47-Lansdowne'/>
<route tag='48' title='48-Rathburn'/>
<route tag='49' title='49-Bloor West'/>
<route tag='50' title='50-Burnhamthorpe'/>
<route tag='51' title='51-Leslie'/>
<route tag='52' title='52-Lawrence West'/>
<route tag='53' title='53-Steeles East'/>
<route tag='54' title='54-Lawrence East'/>
<route tag='55' title='55-Warren Park'/>
<route tag='56' title='56-Leaside'/>
<route tag='57' title='57-Midland'/>
<route tag='59' title='59-Maple Leaf'/>
<route tag='60' title='60-Steeles West'/>
<route tag='61' title='61-Avenue Rd North'/>
<route tag='62' title='62-Mortimer'/>
<route tag='63' title='63-Ossington'/>
<route tag='64' title='64-Main'/>
<route tag='65' title='65-Parliament'/>
<route tag='66' title='66-Prince Edward'/>
<route tag='67' title='67-Pharmacy'/>
<route tag='68' title='68-Warden'/>
<route tag='69' title='69-Warden South'/>
<route tag='70' title='70-O&apos;Connor'/>
<route tag='71' title='71-Runnymede'/>
<route tag='72' title='72-Pape'/>
<route tag='73' title='73-Royal York'/>
<route tag='74' title='74-Mt Pleasant'/>
<route tag='75' title='75-Sherbourne'/>
<route tag='76' title='76-Royal York South'/>
<route tag='77' title='77-Swansea'/>
<route tag='78' title='78-St Andrews'/>
<route tag='79' title='79-Scarlett Rd'/>
<route tag='80' title='80-Queensway'/>
<route tag='81' title='81-Thorncliffe Park'/>
<route tag='82' title='82-Rosedale'/>
<route tag='83' title='83-Jones'/>
<route tag='84' title='84-Sheppard West'/>
<route tag='85' title='85-Sheppard East'/>
<route tag='86' title='86-Scarborough'/>
<route tag='87' title='87-Cosburn'/>
<route tag='88' title='88-South Leaside'/>
<route tag='89' title='89-Weston'/>
<route tag='90' title='90-Vaughan'/>
<route tag='91' title='91-Woodbine'/>
<route tag='92' title='92-Woodbine South'/>
<route tag='94' title='94-Wellesley'/>
<route tag='95' title='95-York Mills'/>
<route tag='96' title='96-Wilson'/>
<route tag='97' title='97-Yonge'/>
<route tag='98' title='98-Willowdale - Senlac'/>
<route tag='99' title='99-Arrow Road'/>
<route tag='100' title='100-Flemingdon Park'/>
<route tag='101' title='101-Downsview Park'/>
<route tag='102' title='102-Markham Rd'/>
<route tag='103' title='103-Mt Pleasant North'/>
<route tag='104' title='104-Faywood'/>
<route tag='105' title='105-Dufferin North'/>
<route tag='106' title='106-York University'/>
<route tag='107' title='107-Keele North'/>
<route tag='108' title='108-Downsview'/>
<route tag='109' title='109-Ranee'/>
<route tag='110' title='110-Islington South'/>
<route tag='111' title='111-East Mall'/>
<route tag='112' title='112-West Mall'/>
<route tag='113' title='113-Danforth'/>
<route tag='115' title='115-Silver Hills'/>
<route tag='116' title='116-Morningside'/>
<route tag='117' title='117-Alness'/>
<route tag='120' title='120-Calvington'/>
<route tag='122' title='122-Graydon Hall'/>
<route tag='123' title='123-Shorncliffe'/>
<route tag='124' title='124-Sunnybrook'/>
<route tag='125' title='125-Drewry'/>
<route tag='126' title='126-Christie'/>
<route tag='127' title='127-Davenport'/>
<route tag='129' title='129-McCowan North'/>
<route tag='130' title='130-Middlefield'/>
<route tag='131' title='131-Nugget'/>
<route tag='132' title='132-Milner'/>
<route tag='133' title='133-Neilson'/>
<route tag='134' title='134-Progress'/>
<route tag='135' title='135-Gerrard'/>
<route tag='139' title='139-Finch-Don Mills'/>
<route tag='141' title='141-Downtown/Mt Pleasant Express'/>
<route tag='142' title='142-Downtown/Avenue Rd Express'/>
<route tag='143' title='143-Downtown/Beach Express'/>
<route tag='144' title='144-Downtown/Don Valley Express'/>
<route tag='145' title='145-Downtown/Humber Bay Express'/>
<route tag='160' title='160-Bathurst North'/>
<route tag='161' title='161-Rogers Rd'/>
<route tag='162' title='162-Lawrence - Donway'/>
<route tag='165' title='165-Weston Rd North'/>
<route tag='167' title='167-Pharmacy North'/>
<route tag='168' title='168-Symington'/>
<route tag='169' title='169-Huntingwood'/>
<route tag='171' title='171-Mount Dennis'/>
<route tag='172' title='172-Cherry Street'/>
<route tag='190' title='190-Scarborough Centre Rocket'/>
<route tag='191' title='191-Highway 27 Rocket'/>
<route tag='192' title='192-Airport Rocket'/>
<route tag='195' title='195-Jane Rocket'/>
<route tag='196' title='196-York University Rocket'/>
<route tag='198' title='198-U Of T Scarborough Rocket'/>
<route tag='199' title='199-Finch Rocket'/>
<route tag='224' title='224-Victoria Park North'/>
<route tag='300' title='300-Bloor - Danforth'/>
<route tag='301' title='301-Queen'/>
<route tag='302' title='302-Danforth Rd - McCowan'/>
<route tag='304' title='304-King'/>
<route tag='306' title='306-Carlton'/>
<route tag='310' title='310-Bathurst'/>
<route tag='312' title='312-St Clair-Junction Night Bus'/>
<route tag='315' title='315-Evans - Brown&apos;S Line'/>
<route tag='317' title='317-Spadina N.C.'/>
<route tag='320' title='320-Yonge'/>
<route tag='322' title='322-Coxwell'/>
<route tag='324' title='324-Victoria Park'/>
<route tag='325' title='325-Don Mills'/>
<route tag='329' title='329-Dufferin'/>
<route tag='332' title='332-Eglinton West'/>
<route tag='334' title='334-Eglinton East'/>
<route tag='336' title='336-Finch West'/>
<route tag='337' title='337-Islington'/>
<route tag='339' title='339-Finch East'/>
<route tag='341' title='341-Keele'/>
<route tag='343' title='343-Kennedy'/>
<route tag='352' title='352-Lawrence West'/>
<route tag='353' title='353-Steeles'/>
<route tag='354' title='354-Lawrence East'/>
<route tag='363' title='363-Ossington'/>
<route tag='365' title='365-Parliament'/>
<route tag='384' title='384-Sheppard West'/>
<route tag='385' title='385-Sheppard East'/>
<route tag='395' title='395-York Mills'/>
<route tag='396' title='396-Wilson'/>
<route tag='501' title='501-Queen'/>
<route tag='502' title='502-Downtowner'/>
<route tag='503' title='503-Kingston Rd'/>
<route tag='504' title='504-King'/>
<route tag='505' title='505-Dundas'/>
<route tag='506' title='506-Carlton'/>
<route tag='509' title='509-Harbourfront'/>
<route tag='510' title='510-Spadina'/>
<route tag='511' title='511-Bathurst'/>
<route tag='512' title='512-St Clair'/>
</body>
",
    _al = @"<?xml version='1.0' encoding='utf-8' ?> 
<body copyright='All data copyright agencies listed below and NextBus Inc 2015.'>
<agency tag='actransit' title='AC Transit' regionTitle='California-Northern'/>
<agency tag='jhu-apl' title='APL' regionTitle='Maryland'/>
<agency tag='art' title='Asheville Redefines Transit' regionTitle='North Carolina'/>
<agency tag='brockton' title='Brockton Area Transit Authority' regionTitle='Massachusetts'/>
<agency tag='camarillo' title='Camarillo Area (CAT)' shortTitle='Camarillo (CAT)' regionTitle='California-Southern'/>
<agency tag='ccrta' title='Cape Cod Regional Transit Authority' shortTitle='CCRTA' regionTitle='Massachusetts'/>
<agency tag='chapel-hill' title='Chapel Hill Transit' shortTitle='Chapel Hill' regionTitle='North Carolina'/>
<agency tag='charles-river' title='Charles River TMA - EZRide' shortTitle='Charles River EZRide' regionTitle='Massachusetts'/>
<agency tag='charm-city' title='Charm City Circulator' regionTitle='Maryland'/>
<agency tag='ccny' title='City College NYC' regionTitle='New York'/>
<agency tag='oxford-ms' title='City of Oxford' regionTitle='Mississippi'/>
<agency tag='collegetown' title='Collegetown Shuttle' regionTitle='Maryland'/>
<agency tag='cyride' title='CyRide' regionTitle='Iowa'/>
<agency tag='dc-circulator' title='DC Circulator' regionTitle='District of Columbia'/>
<agency tag='da' title='Downtown Connection' regionTitle='New York'/>
<agency tag='dumbarton' title='Dumbarton Express' shortTitle='Dumbarton Exp' regionTitle='California-Northern'/>
<agency tag='ecu' title='East Carolina University' regionTitle='North Carolina'/>
<agency tag='emery' title='Emery-Go-Round' regionTitle='California-Northern'/>
<agency tag='fairfax' title='Fairfax (CUE)' regionTitle='Virginia'/>
<agency tag='foothill' title='Foothill Transit' regionTitle='California-Southern'/>
<agency tag='ft-worth' title='Fort Worth The T' regionTitle='Texas'/>
<agency tag='georgia-college' title='Georgia College' regionTitle='Georgia'/>
<agency tag='glendale' title='Glendale Beeline' regionTitle='California-Southern'/>
<agency tag='south-coast' title='Gold Coast Transit' regionTitle='California-Southern'/>
<agency tag='indianapolis-air' title='Indianapolis International Airport' regionTitle='Indiana'/>
<agency tag='jtafla' title='Jacksonville Transportation Authority' shortTitle='Jacksonville' regionTitle='Florida'/>
<agency tag='lasell' title='Lasell College' regionTitle='Massachusetts'/>
<agency tag='lametro' title='Los Angeles Metro' regionTitle='California-Southern'/>
<agency tag='lametro-rail' title='Los Angeles Rail' regionTitle='California-Southern'/>
<agency tag='mbta' title='MBTA' regionTitle='Massachusetts'/>
<agency tag='mit' title='Massachusetts Institute of Technology' shortTitle='MIT' regionTitle='Massachusetts'/>
<agency tag='sf-mission-bay' title='Mission Bay' regionTitle='California-Northern'/>
<agency tag='moorpark' title='Moorpark Transit' regionTitle='California-Southern'/>
<agency tag='bronx' title='NYC MTA - Bronx' regionTitle='New York'/>
<agency tag='brooklyn' title='NYC MTA - Brooklyn' regionTitle='New York'/>
<agency tag='staten-island' title='NYC MTA - Staten Island' regionTitle='New York'/>
<agency tag='nctd' title='North County Transit District' shortTitle='NCTD' regionTitle='California-Southern'/>
<agency tag='nova-se' title='Nova Southeastern University' shortTitle='Nova' regionTitle='Florida'/>
<agency tag='omnitrans' title='Omnitrans' regionTitle='California-Southern'/>
<agency tag='pvpta' title='Palos Verdes Transit' regionTitle='California-Southern'/>
<agency tag='sria' title='Pensacola Beach (SRIA)' shortTitle='SRIA' regionTitle='Florida'/>
<agency tag='portland-sc' title='Portland Streetcar' regionTitle='Oregon'/>
<agency tag='pgc' title='Prince Georges County' regionTitle='Maryland'/>
<agency tag='reno' title='RTC RIDE, Reno' regionTitle='Nevada'/>
<agency tag='radford' title='Radford Transit' regionTitle='Virginia'/>
<agency tag='howard' title='Regional Transportation Agency of Central Maryland' regionTitle='Maryland'/>
<agency tag='roosevelt' title='Roosevelt Island' regionTitle='New York'/>
<agency tag='rutgers-newark' title='Rutgers Univ. Newark College Town Shuttle' shortTitle='Rutgers Newark Shuttle' regionTitle='New Jersey'/>
<agency tag='rutgers' title='Rutgers University' shortTitle='Rutgers' regionTitle='New Jersey'/>
<agency tag='sf-muni' title='San Francisco Muni' shortTitle='SF Muni' regionTitle='California-Northern'/>
<agency tag='seattle-sc' title='Seattle Streetcar' regionTitle='Washington'/>
<agency tag='simi-valley' title='Simi Valley (SVT)' regionTitle='California-Southern'/>
<agency tag='stl' title='Societe de transport de Laval' shortTitle='Laval' regionTitle='Quebec'/>
<agency tag='thousand-oaks' title='Thousand Oaks Transit (TOT)' shortTitle='Thousand Oaks Transit' regionTitle='California-Southern'/>
<agency tag='thunderbay' title='Thunder Bay' regionTitle='Ontario'/>
<agency tag='ttc' title='Toronto Transit Commission' shortTitle='Toronto TTC' regionTitle='Ontario'/>
<agency tag='unitrans' title='Unitrans ASUCD/City of Davis' shortTitle='Unitrans' regionTitle='California-Northern'/>
<agency tag='ucsf' title='University of California San Francisco' regionTitle='California-Northern'/>
<agency tag='umd' title='University of Maryland' regionTitle='Maryland'/>
<agency tag='umn-twin' title='University of Minnesota' regionTitle='Minnesota'/>
<agency tag='vista' title='Ventura Intercity (VISTA)' shortTitle='Ventura (VISTA)' regionTitle='California-Southern'/>
<agency tag='wku' title='Western Kentucky University' regionTitle='Kentucky'/>
<agency tag='winston-salem' title='Winston-Salem' regionTitle='North Carolina'/>
<agency tag='york-pa' title='York College' regionTitle='Pennsylvania'/>
</body>
";
        #endregion
    }
}
