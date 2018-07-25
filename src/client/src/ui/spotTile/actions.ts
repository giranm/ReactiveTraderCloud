import { ExecuteTradeRequest, ExecuteTradeResponse, RegionSettings } from 'rt-types'
import { action, ActionUnion } from 'rt-util'
import { SpotPriceTick } from './model/spotPriceTick'
import { TradeExectionMeta } from './model/spotTileUtils'

export enum ACTION_TYPES {
  EXECUTE_TRADE = '@ReactiveTraderCloud/EXECUTE_TRADE',
  TRADE_EXECUTED = '@ReactiveTraderCloud/TRADE_EXECUTED',
  UNDOCK_TILE = '@ReactiveTraderCloud/UNDOCK_TILE',
  TILE_UNDOCKED = '@ReactiveTraderCloud/TILE_UNDOCKED',
  DISPLAY_CURRENCY_CHART = '@ReactiveTraderCloud/DISPLAY_CURRENCY_CHART',
  CURRENCY_CHART_OPENED = '@ReactiveTraderCloud/CURRENCY_CHART_OPENED',
  DISMISS_NOTIFICATION = '@ReactiveTraderCloud/DISMISS_NOTIFICATION',
  SHOW_SPOT_TILE = '@ReactiveTraderCloud/SHOW_SPOT_TILE',
  SPOT_PRICES_UPDATE = '@ReactiveTraderCloud/SPOT_PRICES_UPDATE'
}

export const SpotTileActions = {
  executeTrade: action<ACTION_TYPES.EXECUTE_TRADE, ExecuteTradeRequest, TradeExectionMeta | null>(
    ACTION_TYPES.EXECUTE_TRADE
  ),
  tradeExecuted: action<ACTION_TYPES.TRADE_EXECUTED, ExecuteTradeResponse, TradeExectionMeta | null>(
    ACTION_TYPES.TRADE_EXECUTED
  ),
  undockTile: action<ACTION_TYPES.UNDOCK_TILE, string>(ACTION_TYPES.UNDOCK_TILE),
  tileUndocked: action<ACTION_TYPES.TILE_UNDOCKED>(ACTION_TYPES.TILE_UNDOCKED),
  displayCurrencyChart: action<ACTION_TYPES.DISPLAY_CURRENCY_CHART, string>(ACTION_TYPES.DISPLAY_CURRENCY_CHART),
  currencyChartOpened: action<ACTION_TYPES.CURRENCY_CHART_OPENED, string>(ACTION_TYPES.CURRENCY_CHART_OPENED),
  dismissNotification: action<ACTION_TYPES.DISMISS_NOTIFICATION, string>(ACTION_TYPES.DISMISS_NOTIFICATION),
  showSpotTile: action<ACTION_TYPES.SHOW_SPOT_TILE, string>(ACTION_TYPES.SHOW_SPOT_TILE),
  priceUpdateAction: action<ACTION_TYPES.SPOT_PRICES_UPDATE, SpotPriceTick>(ACTION_TYPES.SPOT_PRICES_UPDATE)
}

export type SpotTileActions = ActionUnion<typeof SpotTileActions>

export const spotRegionSettings = (id: string): RegionSettings => ({
  title: `${id} Spot`,
  width: 370,
  height: 155,
  dockable: true,
  resizable: false
})
