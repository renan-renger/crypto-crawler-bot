# -*- coding: utf-8 -*-

# Define here the models for your scraped items
#
# See documentation in:
# https://doc.scrapy.org/en/latest/topics/items.html

import scrapy


class CryptoScrapItem(scrapy.Item):
    # define the fields for your item here like:
    source = scrapy.Field()
    endpoint = scrapy.Field()
    isScraped = scrapy.Field()
    collectedAt = scrapy.Field()
    payload = scrapy.Field()