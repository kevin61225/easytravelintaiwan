-- phpMyAdmin SQL Dump
-- version 3.5.1
-- http://www.phpmyadmin.net
--
-- 主机: localhost
-- 生成日期: 2013 年 08 月 22 日 05:21
-- 服务器版本: 5.5.24-log
-- PHP 版本: 5.3.13

SET SQL_MODE="NO_AUTO_VALUE_ON_ZERO";
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8 */;

--
-- 数据库: `project`
--

-- --------------------------------------------------------

--
-- 表的结构 `travellistplace`
--

CREATE TABLE IF NOT EXISTS `travellistplace` (
  `Tid` int(11) NOT NULL,
  `Sno` text NOT NULL,
  KEY `Tid` (`Tid`),
  KEY `Tid_2` (`Tid`)
) ENGINE=InnoDB DEFAULT CHARSET=big5;

--
-- 限制导出的表
--

--
-- 限制表 `travellistplace`
--
ALTER TABLE `travellistplace`
  ADD CONSTRAINT `travellistplace_ibfk_1` FOREIGN KEY (`Tid`) REFERENCES `travellist` (`Tid`);

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
