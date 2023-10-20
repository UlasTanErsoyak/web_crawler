import pyodbc
import csv
from pyvis.network import Network
import networkx as nx
import sys
import webbrowser
import datetime
def get_color_for_spider_id(spider_id):
    spider_id_colors = {
        "1": "red",
        "2": "blue",
        "3": "green",
        "4": "orange",
        "5": "purple",
        "6": "yellow",
        "7": "cyan",
        "8": "magenta",
        "9": "lime",
        "10": "pink",
    }
    return spider_id_colors.get(spider_id, "white")
def connect_to_ssms(tableName):
    try:
        print(tableName)
        server_name = "DESKTOP-V6IUBGJ\\MSSQLSERVER01"
        database_name = "WebCrawler"
        conn_str = f"DRIVER={{ODBC Driver 17 for SQL Server}};SERVER={server_name};DATABASE={database_name};Trusted_Connection=yes;"
        conn = pyodbc.connect(conn_str)
        print("Connected to SQL Server.")
        cursor = conn.cursor()
        query = f"SELECT * FROM {tableName};"
        cursor.execute(query)
        rows = cursor.fetchall()
        cursor.close()
        conn.close()
        visualize_tree(rows)
    except pyodbc.Error as ex:
        print("Error connecting to SQL Server:", ex)
def traverse_graph_dfs(graph, node_id, spider_id_colors, visited, data):
    if node_id not in visited:
        visited.add(node_id)
        for neighbor in graph.neighbors(node_id):
            traverse_graph_dfs(graph, neighbor, spider_id_colors, visited, data)
        color = spider_id_colors.get(int(node_id), "white")
        if 1 <= int(node_id) <= len(data):
            title = f"URLID: {node_id}\nParentID: {data[int(node_id)-1][1]}\nDepth: {data[int(node_id)-1][2]}\nSpiderID: {data[int(node_id)-1][3]}\nCreatedURLCount: {data[int(node_id)-1][4]}\nURLAddress: {data[int(node_id)-1][5]}\nFoundingDate: {data[int(node_id)-1][6]}\nCrawlingDate: {data[int(node_id)-1][7]}\nIsFailed: {data[int(node_id)-1][8]}"
            graph.nodes[node_id]["color"] = color
            graph.nodes[node_id]["title"] = title
def visualize_tree(data):
    parents = {}
    spider_id_colors = {} 
    for row in data:
        node_id = str(row[0])
        parent_id = str(row[1])
        spider_id = str(row[3])
        parents[node_id] = parent_id
        spider_id_colors[int(node_id)] = get_color_for_spider_id(spider_id)
    graph = nx.DiGraph()
    for row in data:
        node_id = str(row[0])
        node_label = f"URLID: {row[0]}\nSpider ID: {row[3]}"
        node_info = get_node_info(row)
        graph.add_node(node_id, label=node_label, title=node_info)
    for node_id in graph.nodes:
        graph.nodes[node_id]["color"] = "white"
    for node_id, parent_id in parents.items():
        if parent_id != '-1':
            graph.add_edge(parent_id, node_id)
    pos = nx.spring_layout(graph, seed=42)
    net = Network(
        notebook=True,
        directed=True,
        height="1000px",
        width="100%",
        bgcolor="#222222",
        font_color="white",
        cdn_resources='remote')
    for node_id, node_data in graph.nodes(data=True):
        title = node_data.get("label", "")
        net.add_node(
            node_id,
            label=title,
            x=pos[node_id][0]*100,
            y=-pos[node_id][1]*100,
            title=title,
            color=spider_id_colors.get(int(node_id), "white"))
    for source, target, edge_data in graph.edges(data=True):
        net.add_edge(source, target)
    net.show_buttons(filter_=['physics'])
    net.show("tree_visualization.html")
    webbrowser.open("tree_visualization.html")
def get_node_info(row):
    founding_date = row[6].strftime('%Y-%m-%d %H:%M:%S') if isinstance(row[6], datetime.datetime) else str(row[6])
    crawling_date = row[7].strftime('%Y-%m-%d %H:%M:%S') if isinstance(row[7], datetime.datetime) else str(row[7])
    info_str = (
        f"URLID: {row[0]}\n"
        f"ParentID: {row[1]}\n"
        f"Depth: {row[2]}\n"
        f"SpiderID: {row[3]}\n"
        f"CreatedURLCount: {row[4]}\n"
        f"URLAddress: {row[5]}\n"
        f"FoundingDate: {founding_date}\n"
        f"CrawlingDate: {crawling_date}\n"
        f"IsFailed: {row[8]}")
    return info_str
def main(table_name_from_wpf):
    connect_to_ssms(table_name_from_wpf)
if __name__ == '__main__':
    if len(sys.argv) > 1:
        table_name_from_wpf = sys.argv[1]
        main(table_name_from_wpf)
