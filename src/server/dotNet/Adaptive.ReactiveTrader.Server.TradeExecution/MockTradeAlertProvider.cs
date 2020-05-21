using System;
using RestSharp;
using Serilog;
using Newtonsoft.Json;

namespace Adaptive.ReactiveTrader.Server.TradeExecution
{
  public class MockTradeAlertProvider
  {

    public string pdEventsAPI = "https://events.pagerduty.com/v2/enqueue";
    public string pdRoutingKey = "R02DN32Y334TR3TDRSF9Y0Y890KEEF5I";
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
        'dedup_key': 'TraderDuty_new_relic_123_trade_engine_down',
        'event_action': 'trigger',
        'client': 'TraderDuty',
        'client_url': 'https://fx.traderduty.com',
        'payload': {{
          'summary': 'Critical: Execution Management System Down (Connection pool exceeded)',
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
        'dedup_key': 'TraderDuty_new_relic_123_trade_engine_latency',
        'event_action': 'trigger',
        'client': 'TraderDuty',
        'client_url': 'https://fx.traderduty.com',
        'payload': {{
          'summary': 'Info: Trade Latency Breached >50ms (ID: p28ya843io)',
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
        'dedup_key': 'TraderDuty_new_relic_123_trade_engine_timeout',
        'event_action': 'trigger',
        'client': 'TraderDuty',
        'client_url': 'https://fx.traderduty.com',
        'payload': {{
          'summary': 'Warning: Rejected Trade - Execution Management System Timeout (ID: a98db973kw)',
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
        'dedup_key': 'TraderDuty_zabbix_123',
        'event_action': 'trigger',
        'client': 'TraderDuty',
        'client_url': 'https://fx.traderduty.com',
        'payload': {{
          'summary': 'Warning: System.OutOfMemoryException: Insufficient memory to continue the execution of the program',
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
        'dedup_key': 'TraderDuty_prometheus_123',
        'event_action': 'trigger',
        'client': 'TraderDuty',
        'client_url': 'https://fx.traderduty.com',
        'payload': {{
          'summary': 'Warning: Low trading volume detected: Under $1 Mio USD across 5 minute period',
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

    public void MockHighCPU()
    {
      var rawJson = @"
      {{
        'routing_key': '{0}',
        'dedup_key': 'TraderDuty_datadog_high_cpu',
        'event_action': 'trigger',
        'client': 'TraderDuty',
        'client_url': 'https://fx.traderduty.com',
        'payload': {{
          'summary': 'Warning: Increased Response Time Detected - Avg 9595ms | 90% CPU',
          'source': 'Datadog',
          'severity': 'warning',
          'component': 'tradeexecution',
          'group': 'prod',
          'class': 'helpdesk',
          'custom_details': {{
            'tags': [
              'aws-eks-prod',
              'pd_az:us-west-2c',
              'production',
              'mariadb'
            ]
          }}
        }}
      }}";

      var rawJsonUpdated = string.Format(rawJson, pdRoutingKey);
      dynamic jsonPayload = JsonConvert.DeserializeObject(rawJsonUpdated);
      var jsonStringPayload = JsonConvert.SerializeObject(jsonPayload);

      Log.Warning("Triggering Alert: " + jsonStringPayload);
      TriggerAlert(jsonStringPayload);
    }

    public void MockCloudWatchBill()
    {
      var rawJson = @"
      {{
        'routing_key': '{0}',
        'dedup_key': 'TraderDuty_aws_cloudwatch',
        'event_action': 'trigger',
        'client': 'TraderDuty',
        'client_url': 'https://fx.traderduty.com',
        'payload': {{
          'summary': 'Warning: Build 0.1.11-5-g0c85fbc delayed; estimated charges breached (+10 USD)',
          'source': 'AWS CloudWatch',
          'severity': 'warning',
          'component': 'broker',
          'group': 'uat',
          'class': 'build-metrics',
          'custom_details': {{
            'Unit': 'null',
            'TreatMissingData': '- TreatMissingData:  missing',
            'Threshold': '10',
            'StatisticType': 'Statistic',
            'Statistic': 'MAXIMUM',
            'Region': 'US East (N. Virginia)',
            'Period': '21600',
            'OldStateValue': 'OK',
            'NewStateValue': 'ALARM',
            'NewStateReason': 'Threshold Crossed: 1 out of the last 1 datapoints [10.08 (18/05/19 10:17:00)] was greater than or equal to the threshold (10.0) (minimum 1 datapoint for OK -> ALARM transition).',
            'Namespace': 'AWS/Billing',
            'MetricName': 'EstimatedCharges',
            'EvaluationPeriods': '1',
            'EvaluateLowSampleCountPercentile': '',
            'Dimensions': [
              {{
                'value': 'USD',
                'name': 'Currency'
              }}
            ],
            'ComparisonOperator': 'GreaterThanOrEqualToThreshold',
            'AlarmName': 'TempTest',
            'AlarmDescription': 'null',
            'AWSAccountId': '593311969811'
          }}
        }},
        'images': [
          {{
            'src': 'https://user-images.githubusercontent.com/9296832/47300570-189ae480-d61d-11e8-9f7d-d20de614477c.png',
            'href': 'https://acme.pagerduty.com'
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
