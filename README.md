![web crawler demo](https://example.com/demo.gif)

## table of contents
- [features](#features)
- [visualization](#visualization)
- [installation](#installation)
- [usage](#usage)
- [customization](#customization)
- [database structure](#database-structure)
- [license](#license)
  
### features

- **multi-threaded crawling**: the web crawler utilizes up to 10 threads to maximize crawling efficiency.
- **live url display**: each thread shows its own urls in real-time on labeled panels.
- **crawling modes**: users can choose between vertical or horizontal crawling using stack or queue.
- **customizable settings**: users can adjust crawling depth, number of urls to crawl, and delays between http requests.
- **built-in visualization**: the crawler includes an embedded pyvis python script to visualize the crawled urls in a natural tree shape.
- **database integration**: the crawler creates sql tables for each root url and stores data about the crawled urls, including parentid, depth, spiderid, createdurlcount, urladdress, foundingdate, crawlingdate, and isfailed.
- **table management**: tables can be deleted as needed to manage crawled data efficiently.
- **web browser integration**: users can inspect each crawled url and navigate to it using a button click.
- **custom thread count**: users can choose the number of threads to use during crawling.
- **csv file**: users can download the crawled urls as a csv file.
- 
### visualization

the crawled urls can be visualized using the built-in pyvis script, generating a natural tree-shaped graph that shows the relationships between the urls. example image uses vertical crawling as its crawling direction.

![visualization](https://github.com/UlasTanErsoyak/web_crawler/assets/92662728/a767147b-f697-4dad-9bfb-408d56b818d9)

#### installation

1. clone this repository to your local machine.
```
git clone https://github.com/yourusername/web-crawler.git
cd web-crawler
```

2. change the python executable path in the  historywindow.xaml.cs file
```c#
private async task startpythonscriptasync(string tablename)
        {
            string pythonexepath = @"path_to_pyton.exe";
            string pythonscriptpath = @"tree_visualize.py";
            string pythonscriptpathquoted = $"\"{pythonscriptpath}\"";
            ///rest of the code...
        }
```

3. build and run the wpf application using visual studio or any compatible ide.

### usage

1. launch the web crawler application.

2. enter the starting url(s) and select crawling options (vertical/horizontal, stack/queue, etc.).

3. customize other crawling settings as needed, such as crawling depth, number of urls to crawl, and delays.

4. click on the "start crawling" button to initiate the crawling process with the specified number of threads.

5. monitor the live url display panels to see the progress of each thread.

6. once crawling is complete, the data will be stored in the sql database, and the visualization will be generated using pyvis.

### customization

the web crawler offers several customization options:

- crawling depth: set the depth of the crawling process to control how far the crawler follows links from the initial urls.
- url limit: define the maximum number of urls the crawler should scrape during the crawling process.
- delays: adjust the delay between each http request to prevent overwhelming the target server.
- thread count: choose the number of threads to use for the crawling process.

### database structure

the sql database will be automatically generated and contain the following columns for each crawled url:

- parentid: the id of the parent url (if any) from which the current url was discovered.
- depth: the depth level of the current url from the root url.
- spiderid: the id of the spider/thread responsible for crawling this url.
- createdurlcount: the total count of urls discovered from that url.
- urladdress: the url of the current crawled page.
- foundingdate: the date when the url was discovered.
- crawlingdate: the date when the url was crawled.
- isfailed: indicates whether the crawling of this url failed or succeeded.

## license

this project is licensed under the [MIT license](https://opensource.org/licenses/MIT). feel free to use and modify the code as per the terms of the license.
