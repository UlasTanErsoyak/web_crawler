![web crawler demo](https://example.com/demo.gif)

## table of contents
- [features](#features)
- [visualization](#visualization)
- [customization](#customization)
  
### features

- **multi-threaded crawling**: the web crawler utilizes up to 10 threads to maximize crawling efficiency.
- **live url display**: each thread shows its own urls in real-time on labeled panels.
- **crawling modes**: users can choose between vertical or horizontal crawling using stack or queue.
- **customizable settings**: users can adjust crawling depth, number of urls to crawl, and delays between http requests.
- **built-in visualization**: the crawler includes an embedded pyvis python script to visualize the crawled urls in a natural tree shape.
- **database integration**: the crawler creates sql tables for each root url and stores data about the crawled urls, including parent id, depth, spider id, created url count, url address, founding date, crawling date, and is failed.
- **table management**: tables can be deleted as needed to manage crawled data efficiently.
- **web browser integration**: users can inspect each crawled url and navigate to it using a button click.
- **custom thread count**: users can choose the number of threads to use during crawling.
- **csv file**: users can download the crawled urls as a csv file.
  
### visualization

the crawled urls can be visualized using the built-in pyvis script, generating a natural tree-shaped graph that shows the relationships between the urls. example image uses vertical crawling as its crawling direction.

![visualization](https://github.com/UlasTanErsoyak/web_crawler/assets/92662728/a767147b-f697-4dad-9bfb-408d56b818d9)

### customization

the web crawler offers several customization options:

- crawling depth: set the depth of the crawling process to control how far the crawler follows links from the initial urls. 
- url limit: define the maximum number of urls the crawler should scrape during the crawling process.
- delays: adjust the delay between each http request to prevent overwhelming the target server.
- thread count: choose the number of threads to use for the crawling process.
