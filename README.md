# Multi-Threaded WPF Web Crawler with Visualization

![Web Crawler Demo](https://example.com/demo.gif)

## Table of Contents
- [Introduction](#introduction)
- [Features](#features)
- [Installation](#installation)
- [Usage](#usage)
- [Customization](#customization)
- [Database Structure](#database-structure)
- [Visualization](#visualization)
- [Contributing](#contributing)
- [License](#license)

## Introduction

This is a powerful multi-threaded WPF web crawler built to efficiently scrape and visualize URLs from the web. It allows users to customize crawling behavior, set crawling depth, number of URLs to crawl, delays between HTTP requests, and more. The crawler can operate in both vertical and horizontal modes using stack or queue, depending on user preferences.

## Features

- **Multi-Threaded Crawling**: The web crawler utilizes up to 10 threads to maximize crawling efficiency.
- **Live URL Display**: Each thread shows its own URLs in real-time on labeled panels.
- **Crawling Modes**: Users can choose between vertical or horizontal crawling using stack or queue.
- **Customizable Settings**: Users can adjust crawling depth, number of URLs to crawl, and delays between HTTP requests.
- **Built-in Visualization**: The crawler includes an embedded Pyvis Python script to visualize the crawled URLs in a natural tree shape.
- **Database Integration**: The crawler creates SQL tables for each root URL and stores data about the crawled URLs, including ParentID, Depth, SpiderID, CreatedURLCount, URLAddress, FoundingDate, CrawlingDate, and IsFailed.
- **Table Management**: Tables can be deleted as needed to manage crawled data efficiently.
- **Web Browser Integration**: Users can inspect each crawled URL and navigate to it using a button click.
- **Custom Thread Count**: Users can choose the number of threads to use during crawling.

## Installation

1. Clone this repository to your local machine.
```
git clone https://github.com/yourusername/web-crawler.git
cd web-crawler
```

2. Change the python executable path in the  HistoryWindow.xaml..cs file
```C#
private async Task StartPythonScriptAsync(string tableName)
        {
            string pythonExePath = @"path_to_pyton.exe";
            string pythonScriptPath = @"tree_visualize.py";
            string pythonScriptPathQuoted = $"\"{pythonScriptPath}\"";
            ///rest of the code...
        }
```

3. Build and run the WPF application using Visual Studio or any compatible IDE.

## Usage

1. Launch the web crawler application.

2. Enter the starting URL(s) and select crawling options (vertical/horizontal, stack/queue, etc.).

3. Customize other crawling settings as needed, such as crawling depth, number of URLs to crawl, and delays.

4. Click on the "Start Crawling" button to initiate the crawling process with the specified number of threads.

5. Monitor the live URL display panels to see the progress of each thread.

6. Once crawling is complete, the data will be stored in the SQL database, and the visualization will be generated using Pyvis.

## Customization

The web crawler offers several customization options:

- Crawling Depth: Set the depth of the crawling process to control how far the crawler follows links from the initial URLs.
- URL Limit: Define the maximum number of URLs the crawler should scrape during the crawling process.
- Delays: Adjust the delay between each HTTP request to prevent overwhelming the target server.
- Thread Count: Choose the number of threads to use for the crawling process.

## Database Structure

The SQL database will be automatically generated and contain the following columns for each crawled URL:

- ParentID: The ID of the parent URL (if any) from which the current URL was discovered.
- Depth: The depth level of the current URL from the root URL.
- SpiderID: The ID of the spider/thread responsible for crawling this URL.
- CreatedURLCount: The total count of URLs discovered by the spider/thread.
- URLAddress: The URL of the current crawled page.
- FoundingDate: The date when the URL was discovered.
- CrawlingDate: The date when the URL was crawled.
- IsFailed: Indicates whether the crawling of this URL failed or succeeded.

## Visualization

The crawled URLs can be visualized using the built-in Pyvis script, generating a natural tree-shaped graph that shows the relationships between the URLs. Example image uses vertical crawling as its crawling direction.

![visualization](https://github.com/UlasTanErsoyak/web_crawler/assets/92662728/a767147b-f697-4dad-9bfb-408d56b818d9)

## Contributing

Contributions to the project are welcome! If you find any issues or have ideas for improvements, please open an issue or submit a pull request.

## License

This project is licensed under the [MIT License](https://opensource.org/licenses/MIT). Feel free to use and modify the code as per the terms of the license.
