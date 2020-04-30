using System;
using RestSharp;
using Serilog;
using Newtonsoft.Json;

namespace Adaptive.ReactiveTrader.Server.TradeExecution
{
  public class MockTradeAlertProvider
  {

    public string pdEventsAPI = "https://events.pagerduty.com/v2/enqueue";
    public string pdRoutingKey = "R02097FVAP02WT5HEDYZZ0QXT17Q8NJ6";
    public RestClient client;

    public MockTradeAlertProvider()
    {
      client = new RestClient(pdEventsAPI);
      client.Timeout = -1;
    }

    // Helper methods.
    public void TriggerAlert(string payload)
    {
      var request = new RestRequest(Method.POST);
      request.AddHeader("Content-Type", "application/json");
      request.AddParameter("application/json", payload, ParameterType.RequestBody);
      IRestResponse response = client.Execute(request);
      Log.Information(response.Content);
    }

    public void MockTradeEngineDown()
    {
      var rawJson = @"
      {{
        'routing_key': '{0}',
        'dedup_key': 'new_relic_123_trade_engine_down',
        'event_action': 'trigger',
        'client': 'TraderDuty',
        'client_url': 'https://fx.traderduty.com',
        'payload': {{
          'summary': 'Failure: Trading execution engine down (Connection pool exceeded)',
          'source': 'pod/tradeexecution-64f46d8b94-nrrd4',
          'severity': 'critical',
          'component': 'tradeexecution',
          'group': 'prod',
          'class': 'db-connection',
          'custom_details': {{
            'tags': [
            'aws-eks-prod',
            'pd_az:us-west-2c',
            'production',
            'mariadb'
          ],
            'priority': 'P1',
            'event_id': '8125363154448140714'
          }}
        }},
        'images': [
          {{
            'src': 'https://s3.eu-west-2.amazonaws.com/www.timchinchen.co.uk/DemoImages/NRErrors2.png',
            'href': 'http://www.timchinchen.co.uk/DemoImages/NRErrors1.png',
            'alt': 'Snapshot of metric'
          }}
        ],
        'links': [
          {{
            'text': 'Zoom: Conference URL',
            'href': 'https://pagerduty.zoom.us/j/5080555253'
          }},
          {{
            'text': 'Zoom: Conference Call',
            'href': 'tel:+442036950088,,5080555253#'
          }},
          {{
            'text': 'New Relic: Dashboard',
            'href': 'https://rpm.newrelic.com/accounts/1818495/applications/105833762?tw%5Bend%5D=1518535392&tw%5Bstart%5D=1518528373'
          }}
        ]
      }}";

      var rawJsonUpdated = string.Format(rawJson, pdRoutingKey);
      dynamic jsonPayload = JsonConvert.DeserializeObject(rawJsonUpdated);
      var jsonStringPayload = JsonConvert.SerializeObject(jsonPayload);

      Log.Fatal("Triggering Alert: " + jsonStringPayload);
      TriggerAlert(jsonStringPayload);
    }

    public void MockTradeLatency()
    {
      var rawJson = @"
      {{
        'routing_key': '{0}',
        'dedup_key': 'new_relic_123_trade_engine_latency',
        'event_action': 'trigger',
        'client': 'TraderDuty',
        'client_url': 'https://fx.traderduty.com',
        'payload': {{
          'summary': 'Trade Latency: 50ms Threshold Breached (ID: p28ya843io)',
          'source': 'pod/tradeexecution-64f46d8b94-nrrd4',
          'severity': 'info',
          'component': 'tradeexecution',
          'group': 'prod',
          'class': 'trade-engine',
          'custom_details': {{
            'tags': [
            'aws-eks-prod',
            'pd_az:us-west-2c',
            'production',
            'mariadb'
          ],
            'priority': 'P3',
            'event_id': '6529719154629272914',
            'trade_id': 'p28ya843io',
            'isin': 'EU0009652627',
            'symbol': 'EUR/JPY',
            'trade_process_time': 5639
          }}
        }}
      }}";

      var rawJsonUpdated = string.Format(rawJson, pdRoutingKey);
      dynamic jsonPayload = JsonConvert.DeserializeObject(rawJsonUpdated);
      var jsonStringPayload = JsonConvert.SerializeObject(jsonPayload);

      Log.Warning("Triggering Alert: " + jsonStringPayload);
      TriggerAlert(jsonStringPayload);
    }

    public void MockTradeRejection()
    {
      var rawJson = @"
      {{
        'routing_key': '{0}',
        'dedup_key': 'new_relic_123_trade_engine_timeout',
        'event_action': 'trigger',
        'client': 'TraderDuty',
        'client_url': 'https://fx.traderduty.com',
        'payload': {{
          'summary': 'Trade Rejected: Engine Timeout (ID: a98db973kw)',
          'source': 'pod/tradeexecution-64f46d8b94-nrrd4',
          'severity': 'warning',
          'component': 'tradeexecution',
          'group': 'prod',
          'class': 'trade-engine',
          'custom_details': {{
            'tags': [
            'aws-eks-prod',
            'pd_az:us-west-2c',
            'production',
            'mariadb'
          ],
            'priority': 'P2',
            'event_id': '6235363154446273614',
            'trade_id': 'a98db973kw',
            'isin': 'GB000A161NR7',
            'symbol': 'GBP/JPY',
            'trade_process_time': -1
          }}
        }}
      }}";

      var rawJsonUpdated = string.Format(rawJson, pdRoutingKey);
      dynamic jsonPayload = JsonConvert.DeserializeObject(rawJsonUpdated);
      var jsonStringPayload = JsonConvert.SerializeObject(jsonPayload);

      Log.Warning("Triggering Alert: " + jsonStringPayload);
      TriggerAlert(jsonStringPayload);
    }

    public void MockOutOfMemory()
    {
      var rawJson = @"
      {{
        'routing_key': '{0}',
        'dedup_key': 'zabbix_123',
        'event_action': 'trigger',
        'client': 'TraderDuty',
        'client_url': 'https://fx.traderduty.com',
        'payload': {{
          'summary': 'System.OutOfMemoryException: Insufficient memory to continue the execution of the program',
          'source': 'pod/eventstore-d85c4ddc9-jzlz7',
          'severity': 'warning',
          'component': 'eventstore',
          'group': 'prod',
          'class': 'db-connection',
          'custom_details': {{
            'tags': [
            'aws-eks-prod',
            'pd_az:us-west-2c',
            'production',
            'mariadb'
          ],
            'priority': 'High',
            'event_id': '8158263154448152614'
          }}
        }},
        'images': [
          {{
            'src': 'https://chart.googleapis.com/chart?chs=600x400&chd=t:6,2,9,5,2,5,7,4,8,2,1&cht=lc&chds=a&chxt=y&chm=D,0033FF,0,0,5,1',
            'href': 'https://acme.pagerduty.com',
            'alt': 'This is a sample link'
          }}
        ]
      }}";

      var rawJsonUpdated = string.Format(rawJson, pdRoutingKey);
      dynamic jsonPayload = JsonConvert.DeserializeObject(rawJsonUpdated);
      var jsonStringPayload = JsonConvert.SerializeObject(jsonPayload);

      Log.Warning("Triggering Alert: " + jsonStringPayload);
      TriggerAlert(jsonStringPayload);
    }

    public void MockLowTradingVolume()
    {
      var rawJson = @"
      {{
        'routing_key': '{0}',
        'dedup_key': 'prometheus_123',
        'event_action': 'trigger',
        'client': 'TraderDuty',
        'client_url': 'https://fx.traderduty.com',
        'payload': {{
          'summary': 'Low trading volume detected: Under $1 Mio USD across 5 minute period',
          'source': 'pod/analytics-bf6466bb7-h9rpw',
          'severity': 'warning',
          'component': 'analytics',
          'group': 'prod',
          'class': 'revenue',
          'custom_details': {{
            'tags': [
            'aws-eks-prod',
            'pd_az:us-west-2c',
            'production',
            'mariadb'
          ],
            'priority': 'High',
            'event_id': '8155273154820152614'
          }}
        }},
        'images': [
          {{
            'src': 'https://logz.io/wp-content/uploads/2017/03/memory-graph.png',
            'href': 'https://logz.io/wp-content/uploads/2017/03/memory-graph.png',
            'alt': 'This is a sample link'
          }}
        ]
      }}";

      var rawJsonUpdated = string.Format(rawJson, pdRoutingKey);
      dynamic jsonPayload = JsonConvert.DeserializeObject(rawJsonUpdated);
      var jsonStringPayload = JsonConvert.SerializeObject(jsonPayload);

      Log.Warning("Triggering Alert: " + jsonStringPayload);
      TriggerAlert(jsonStringPayload);
    }

  }
}
