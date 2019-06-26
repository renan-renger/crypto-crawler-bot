import scrapy
import os
import w3lib.url
import time
import json
from scrapy import Selector
from scrapy import Request
from bs4 import BeautifulSoup
from datetime import datetime
from cryptoScrap.items import CryptoScrapItem

class CoinMarketCapSpider(scrapy.Spider):
    name = 'coinMarketCap'

    # We take 3 params: URL, search term and ordering
    # While we could take the category, it's worthless unless we get the right one
    # ML' routing would just pass the category term to the search term and use the old layout
    # Init override allows us to exclude the current file before each to crawler execution to ensure that the data is not stale
    def __init__(self, s=None, e=None):
        self.crawlStart = datetime.now()
        self.fileTemporal = self.crawlStart.strftime('%Y-%m-%d_%H-%M-%S')

        self.start_url = 'https://coinmarketcap.com/currencies/bitcoin/historical-data/'
        self.startDate = s
        self.endDate = e

        self.columnGuide = [
                'timestamp',
                'open',
                'high',
                'low',
                'close',
                'traded_volume',
                'market_cap']

    def start_requests(self):

        prepdUrl = self.start_url

        if (self.startDate and self.endDate):
            prepdUrl = w3lib.url.add_or_replace_parameters(
                self.start_url,
                {
                    'start' : self.startDate,
                    'end' : self.endDate
                }
            )

        yield Request(prepdUrl, callback=self.parse)

    def parse(self, response):

        hxs = Selector(response).xpath('//table[contains(@class,"table")]')
        rows = hxs.xpath('.//tr[contains(@class,"text-right")]')

        tableData = {
            'source' : 'CoinMarketCap',
            'endpoint' : 'https://coinmarketcap.com/currencies/bitcoin/historical-data',
            'isScraped' : True,
            'collectedAt' : self.crawlStart.strftime('%Y-%m-%d %H:%M:%S'),
            'payload' : []
            }

        for i, row in enumerate(rows):

            columns = row.xpath('.//td') 

            rowData = {
                'timestamp' : None,
                'open' : None,
                'high' : None,
                'low' : None,
                'close' : None,
                'traded_volume' : None,
                'market_cap' : None
            }

            for j, column in enumerate(columns):
                soup = BeautifulSoup(column.get(),features="lxml")

                if (j == 0):
                    rowData[self.columnGuide[j]] = datetime.strptime(soup.get_text(), "%b %d, %Y").timestamp()
                else:
                    rowData[self.columnGuide[j]] = float(soup.get_text().replace(',',''))


            tableData['payload'].append(rowData)

        yield tableData