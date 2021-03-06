# Full Node Installer
## Redstone Full Node on Ubuntu

An automated script to install a Redstone Node on various platforms.

* To install a Redstone Node on Ubuntu 16.04 & 18.04 - as <code>sudo su root</code> run the following command:

<code>bash <( curl https://raw.githubusercontent.com/RedstonePlatform/Redstone/master/Scripts/Node-Installer/install_redstone_node.sh )</code>

## Redstone Full Node on Raspberry Pi

* To install a  Node on a Raspberry Pi running Raspian - as <code>sudo su root</code> run the following:

<code>bash <( curl https://raw.githubusercontent.com/RedstonePlatform/Redstone/master/Scripts/Node-Installer/install_redstone_RPI.sh )</code>

<code>bash <( curl https://raw.githubusercontent.com/RedstonePlatform/Redstone/master/Scripts/Node-Installer/install_redstone_RPI_binaries.sh )</code>

*If you get the error "bash: curl: command not found", run this first: <code>apt-get -y install curl</code>*

## Redstone Full Node on CentOS

* To install a Node on CentOS - as <code>sudo su root</code> run the following:

<code>bash <( curl https://raw.githubusercontent.com/RedstonePlatform/Redstone/master/Scripts/Node-Installer/install_redstone_CentOS.sh )</code>

## Redstone Full Node with Explorer

In addition to the Redstone Full Node installs MongoDB, Nako Block Chain Indexer, nginx and Stratis.Guru Block Explorer.

<code>bash <( curl https://raw.githubusercontent.com/RedstonePlatform/Redstone/master/Scripts/Node-Installer/install_redstone_explorer_node.sh )</code>

