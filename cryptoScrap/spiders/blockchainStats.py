import scrapy
from scrapy import Selector
from scrapy import Request
import os
import w3lib.url
from bs4 import BeautifulSoup
from datetime import datetime
import time
import json
from pymongo import MongoClient

class MLParamsSpider(scrapy.Spider):
    name = 'blockchainStats'

    # We take 3 params: URL, search term and ordering
    # While we could take the category, it's worthless unless we get the right one
    # ML' routing would just pass the category term to the search term and use the old layout
    # Init override allows us to exclude the current file before each to crawler execution to ensure that the data is not stale
    def __init__(self, s=None, e=None):
        self.crawlStart = datetime.now()
        self.fileTemporal = self.crawlStart.strftime('%Y-%m-%d_%H-%M-%S')

        self.fileName = f'blockchainStats_{self.fileTemporal}.json'

        if os.path.exists(self.fileName):
                os.remove(self.fileName)

        self.start_url = 'https://www.blockchain.com/en/stats'
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
        
    def setupMongoConnection(self):
        self.client = MongoClient('mongodb://scrapper-temp-storage:HWwwZpghIzn1KHQuh1haaeIsgsnz3xg7dpMwXCB4gTGNtd0it2tU2kqWz22p07G2IRIhD32Nl9DhzbA531e1Ug==@scrapper-temp-storage.documents.azure.com:10255/?ssl=true&replicaSet=globaldb')
        db = self.client.admin
        # Issue the serverStatus command and print the results
        serverStatusResult = db.command("serverStatus")
        print(serverStatusResult)

    def start_requests(self):

        self.setupMongoConnection()

        yield Request(self.start_url, callback=self.parse)

    def parse(self, response):
        time.sleep(5)
        json_file = open(self.fileName, 'a+', encoding="utf-8")

        baseRegion = Selector(response).xpath('//div[@id="statstable"]')
        tables = baseRegion.xpath('.//div[not(contains(@class,"links"))]')

        tableData = {
            'source' : 'Blockchain.info',
            'endpoint' : self.start_url,
            'isScraped' : True,
            'collectedAt' : self.crawlStart.strftime('%Y-%m-%d %H:%M:%S'),
            'payload' : []
            }

        for i, table in enumerate(tables):
            rows = table.xpath('.//tr')

            for i, row in enumerate(rows):

                dataColumn = row.xpath('.//td')[1]

                rowData = {
                    'timestamp' : self.crawlStart.strftime('%Y-%m-%d %H:%M:%S'),
                    'key' : dataColumn.css('::attr(id)').extract_first(), #BeautifulSoup(columns[0].get(), features="lxml").get_text(),
                    'value' : dataColumn.css('::attr(innerHTML)').extract_first()
                }

                # for j, column in enumerate(columns):
                #     soup = BeautifulSoup(column.get(),features="lxml")

                #     if (j == 0):
                #         rowData[self.columnGuide[j]] = datetime.strptime(soup.get_text(), "%b %d, %Y").timestamp()
                #     else:
                #         rowData[self.columnGuide[j]] = float(soup.get_text().replace(',',''))


                tableData['payload'].append(rowData)

        scrappedPayload = json.dumps(tableData, indent=4)

        json_file.write(scrappedPayload)

        json_file.close()

        # coldStorage = self.client.crypto_cold_storage

        # currentDocument = coldStorage.cryptoColdStorage.find_one({'source' : 'Blockchain.info'})

        # if (currentDocument is None):
        #     coldStorage.cryptoColdStorage.insert_one(tableData)
        # else:
        #     tableData['payload'] = self.mergeStorageData(currentDocument['payload'], tableData['payload'])
        #     coldStorage.cryptoColdStorage.replace_one({'source' : 'Blockchain.info'}, tableData)
        

    def mergeStorageData(self, currentList, inboundList):

        currentTimestamps = [x['timestamp'] for x in currentList]
        inboundTimestamps = [x['timestamp'] for x in inboundList]

        if (currentTimestamps == inboundTimestamps):
            return inboundList

        mergedTimestamps = list(set(currentTimestamps + inboundTimestamps))

        returnList = []
        found = False

        for mergedTimestamp in mergedTimestamps:

            for item in currentList:
                if item['timestamp'] == mergedTimestamp:
                    returnList.append(item)
                    found = True
                    break

            if (found == False):
                for item in inboundList:
                    if item['timestamp'] == mergedTimestamp:
                        returnList.append(item)
                        break

            found = False

        return returnList