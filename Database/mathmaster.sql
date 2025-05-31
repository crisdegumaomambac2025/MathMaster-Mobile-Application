-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Host: 127.0.0.1
-- Generation Time: May 15, 2025 at 10:27 AM
-- Server version: 10.4.32-MariaDB
-- PHP Version: 8.2.12

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Database: `mathmaster`
--

-- --------------------------------------------------------

--
-- Table structure for table `leaderboard`
--

CREATE TABLE `leaderboard` (
  `id` int(11) NOT NULL,
  `user_id` int(11) NOT NULL,
  `username` varchar(255) NOT NULL,
  `score` varchar(255) NOT NULL,
  `star` int(11) NOT NULL,
  `quiz_name` varchar(255) NOT NULL,
  `date` timestamp NOT NULL DEFAULT current_timestamp() ON UPDATE current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Table structure for table `login_history`
--

CREATE TABLE `login_history` (
  `login_history_id` int(11) NOT NULL,
  `login_time` timestamp NOT NULL DEFAULT current_timestamp(),
  `ip_address` varchar(255) NOT NULL,
  `device_type` varchar(255) NOT NULL,
  `location` varchar(255) NOT NULL,
  `user_id` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `login_history`
--

INSERT INTO `login_history` (`login_history_id`, `login_time`, `ip_address`, `device_type`, `location`, `user_id`) VALUES
(3, '2025-05-01 12:32:44', '112.208.72.125', 'M2102J20SG / Android', 'Lapu-Lapu City, Central Visayas, PH', 6),
(4, '2025-05-01 12:49:12', '112.208.72.125', 'M2102J20SG / Android', 'Lapu-Lapu City, Central Visayas, PH', 6),
(8, '2025-05-01 13:36:01', '112.208.72.125', 'M2102J20SG / Android', 'Lapu-Lapu City, Central Visayas, PH', 6),
(9, '2025-05-01 13:46:13', '112.208.72.125', 'M2102J20SG / Android', 'Lapu-Lapu City, Central Visayas, PH', 6),
(10, '2025-05-01 13:49:24', '112.208.72.125', 'M2102J20SG / Android', 'Lapu-Lapu City, Central Visayas, PH', 6),
(21, '2025-05-01 15:00:28', '112.208.72.125', 'M2102J20SG / Android', 'Lapu-Lapu City, Central Visayas, PH', 6),
(23, '2025-05-01 15:04:21', '112.208.72.125', 'M2102J20SG / Android', 'Lapu-Lapu City, Central Visayas, PH', 6),
(30, '2025-05-01 15:43:28', '112.208.72.125', 'M2102J20SG / Android', 'Lapu-Lapu City, Central Visayas, PH', 5),
(63, '2025-05-02 04:24:39', '112.208.72.125', 'M2102J20SG / Android', 'Lapu-Lapu City, Central Visayas, PH', 5),
(71, '2025-05-02 04:45:00', '112.208.72.125', 'M2102J20SG / Android', 'Lapu-Lapu City, Central Visayas, PH', 5),
(73, '2025-05-02 04:54:57', '112.208.72.125', 'M2102J20SG / Android', 'Lapu-Lapu City, Central Visayas, PH', 5),
(82, '2025-05-02 05:34:41', '112.208.72.125', 'M2102J20SG / Android', 'Lapu-Lapu City, Central Visayas, PH', 5),
(83, '2025-05-02 05:46:20', '112.208.72.125', 'M2102J20SG / Android', 'Lapu-Lapu City, Central Visayas, PH', 5),
(85, '2025-05-02 05:55:45', '112.208.72.125', 'M2102J20SG / Android', 'Lapu-Lapu City, Central Visayas, PH', 5),
(86, '2025-05-02 05:58:29', '112.208.72.125', 'M2102J20SG / Android', 'Lapu-Lapu City, Central Visayas, PH', 5),
(87, '2025-05-02 06:00:06', '112.208.72.125', 'M2102J20SG / Android', 'Lapu-Lapu City, Central Visayas, PH', 6),
(88, '2025-05-02 06:03:53', '112.208.72.125', 'M2102J20SG / Android', 'Lapu-Lapu City, Central Visayas, PH', 5),
(106, '2025-05-02 09:21:24', '110.54.231.109', 'M2102J20SG / Android', 'Cebu City, Central Visayas, PH', 5),
(115, '2025-05-02 11:13:35', '202.90.133.68', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 5),
(208, '2025-05-07 10:57:40', '202.90.133.68', 'sdk_gphone64_x86_64 / Android', 'Quezon City, Metro Manila, PH', 5),
(235, '2025-05-07 18:06:26', '124.217.19.132', 'sdk_gphone64_x86_64 / Android', 'Dapitan, Central Visayas, PH', 5),
(236, '2025-05-07 18:09:13', '124.217.19.132', 'sdk_gphone64_x86_64 / Android', 'Dapitan, Central Visayas, PH', 5),
(237, '2025-05-07 18:11:59', '124.217.19.132', 'sdk_gphone64_x86_64 / Android', 'Dapitan, Central Visayas, PH', 5),
(238, '2025-05-07 18:13:41', '124.217.19.132', 'sdk_gphone64_x86_64 / Android', 'Dapitan, Central Visayas, PH', 5),
(239, '2025-05-07 18:15:37', '124.217.19.132', 'sdk_gphone64_x86_64 / Android', 'Dapitan, Central Visayas, PH', 5),
(240, '2025-05-07 18:18:54', '124.217.19.132', 'sdk_gphone64_x86_64 / Android', 'Dapitan, Central Visayas, PH', 5),
(241, '2025-05-07 18:35:08', '124.217.19.132', 'sdk_gphone64_x86_64 / Android', 'Dapitan, Central Visayas, PH', 5),
(242, '2025-05-07 18:36:08', '124.217.19.132', 'sdk_gphone64_x86_64 / Android', 'Dapitan, Central Visayas, PH', 10),
(249, '2025-05-08 04:04:56', '112.208.67.173', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 5),
(250, '2025-05-08 04:07:44', '112.208.67.173', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 5),
(251, '2025-05-08 04:10:48', '112.208.67.173', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 5),
(252, '2025-05-08 04:10:51', '112.208.67.173', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 5),
(253, '2025-05-08 04:17:10', '112.208.67.173', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 5),
(254, '2025-05-08 04:19:30', '112.208.67.173', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 5),
(257, '2025-05-08 06:56:11', '112.208.67.173', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 5),
(260, '2025-05-08 07:29:36', '112.208.67.173', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 5),
(341, '2025-05-09 03:17:17', '112.208.67.173', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(342, '2025-05-09 03:21:19', '112.208.67.173', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(343, '2025-05-09 03:25:52', '112.208.67.173', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(344, '2025-05-09 03:28:43', '112.208.67.173', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(345, '2025-05-09 03:30:38', '112.208.67.173', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(346, '2025-05-09 03:43:39', '112.208.67.173', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(347, '2025-05-09 03:45:42', '112.208.67.173', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(348, '2025-05-09 03:47:02', '112.208.67.173', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(349, '2025-05-09 03:49:57', '112.208.67.173', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(350, '2025-05-09 03:54:41', '112.208.67.173', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(351, '2025-05-09 04:16:34', '112.208.67.173', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(352, '2025-05-09 04:28:53', '112.208.67.173', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(353, '2025-05-09 04:33:58', '112.208.67.173', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(354, '2025-05-09 04:36:37', '112.208.67.173', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(355, '2025-05-09 04:38:44', '112.208.67.173', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(356, '2025-05-09 04:41:02', '112.208.67.173', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(357, '2025-05-09 04:47:49', '112.208.67.173', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(358, '2025-05-09 04:51:08', '112.208.67.173', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(359, '2025-05-09 04:56:24', '112.208.67.173', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(360, '2025-05-09 04:56:24', '112.208.67.173', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(361, '2025-05-09 04:59:13', '112.208.67.173', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(362, '2025-05-09 05:00:51', '112.208.67.173', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(363, '2025-05-09 05:02:16', '112.208.67.173', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(364, '2025-05-09 05:06:03', '112.208.67.173', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(368, '2025-05-09 05:13:58', '112.208.67.173', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(369, '2025-05-09 05:19:37', '112.208.67.173', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(370, '2025-05-09 05:23:20', '112.208.67.173', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(371, '2025-05-09 05:28:53', '112.208.67.173', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(372, '2025-05-09 05:34:40', '112.208.67.173', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 5),
(373, '2025-05-09 05:48:44', '112.208.67.173', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 5),
(374, '2025-05-09 05:51:48', '112.208.67.173', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 5),
(375, '2025-05-09 05:54:13', '112.208.67.173', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 5),
(376, '2025-05-09 05:57:35', '112.208.67.173', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 5),
(377, '2025-05-09 06:00:54', '112.208.67.173', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 5),
(378, '2025-05-09 06:12:22', '112.208.67.173', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 5),
(379, '2025-05-09 06:15:26', '112.208.67.173', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(380, '2025-05-09 06:19:22', '112.208.67.173', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(381, '2025-05-09 06:22:39', '112.208.67.173', 'M2102J20SG / Android', 'Cebu City, Central Visayas, PH', 14),
(382, '2025-05-09 06:23:12', '112.208.67.173', 'M2102J20SG / Android', 'Cebu City, Central Visayas, PH', 14),
(383, '2025-05-09 06:23:42', '112.208.67.173', 'M2102J20SG / Android', 'Cebu City, Central Visayas, PH', 5),
(384, '2025-05-09 06:24:09', '112.208.67.173', 'M2102J20SG / Android', 'Cebu City, Central Visayas, PH', 14),
(385, '2025-05-09 06:27:02', '112.208.67.173', 'ASUS TUF Gaming F15 FX506LHB_FX506LHB / WinUI', 'Cebu City, Central Visayas, PH', 14),
(386, '2025-05-09 06:27:34', '112.208.67.173', 'M2102J20SG / Android', 'Cebu City, Central Visayas, PH', 14),
(387, '2025-05-09 06:35:33', '112.208.67.173', 'M2102J20SG / Android', 'Cebu City, Central Visayas, PH', 14),
(388, '2025-05-09 06:37:45', '112.208.67.173', 'M2102J20SG / Android', 'Cebu City, Central Visayas, PH', 14),
(389, '2025-05-09 06:42:50', '112.208.67.173', 'M2102J20SG / Android', 'Cebu City, Central Visayas, PH', 14),
(390, '2025-05-09 06:43:31', '112.208.67.173', 'M2102J20SG / Android', 'Cebu City, Central Visayas, PH', 14),
(391, '2025-05-09 07:12:37', '112.208.67.173', 'M2102J20SG / Android', 'Cebu City, Central Visayas, PH', 14),
(392, '2025-05-09 07:15:14', '112.208.67.173', 'M2102J20SG / Android', 'Cebu City, Central Visayas, PH', 14),
(393, '2025-05-09 07:16:34', '112.208.67.173', 'M2102J20SG / Android', 'Cebu City, Central Visayas, PH', 14),
(394, '2025-05-09 07:18:22', '112.208.67.173', 'M2102J20SG / Android', 'Cebu City, Central Visayas, PH', 5),
(395, '2025-05-09 07:20:34', '112.208.67.173', 'M2102J20SG / Android', 'Cebu City, Central Visayas, PH', 14),
(396, '2025-05-09 08:10:18', '112.198.86.0', 'M2102J20SG / Android', 'Cebu City, Central Visayas, PH', 14),
(397, '2025-05-09 08:37:23', '112.198.86.0', 'M2102J20SG / Android', 'Cebu City, Central Visayas, PH', 5),
(398, '2025-05-09 08:39:04', '112.198.86.0', 'M2102J20SG / Android', 'Cebu City, Central Visayas, PH', 14),
(399, '2025-05-09 08:44:15', '112.198.86.0', 'M2102J20SG / Android', 'Cebu City, Central Visayas, PH', 14),
(400, '2025-05-09 14:21:12', '112.208.64.132', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 5),
(401, '2025-05-09 14:36:56', '112.208.64.132', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 5),
(402, '2025-05-09 14:39:37', '112.208.64.132', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 5),
(403, '2025-05-09 14:56:00', '112.208.64.132', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 5),
(404, '2025-05-09 14:59:58', '112.208.64.132', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 5),
(405, '2025-05-09 15:03:39', '112.208.64.132', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 5),
(406, '2025-05-09 15:04:53', '112.208.64.132', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 5),
(407, '2025-05-09 15:08:44', '112.208.64.132', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 5),
(408, '2025-05-09 15:16:24', '112.208.64.132', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 5),
(409, '2025-05-09 15:29:02', '112.208.64.132', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 5),
(410, '2025-05-09 15:33:19', '112.208.64.132', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 5),
(411, '2025-05-09 15:49:29', '112.208.64.132', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 5),
(412, '2025-05-09 15:57:22', '112.208.64.132', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 5),
(413, '2025-05-09 15:57:26', '112.208.64.132', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 5),
(414, '2025-05-09 16:01:32', '112.208.64.132', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 5),
(415, '2025-05-09 16:08:38', '112.208.64.132', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 5),
(416, '2025-05-09 16:12:51', '112.208.64.132', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 5),
(417, '2025-05-09 16:18:24', '112.208.64.132', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 5),
(418, '2025-05-09 16:21:35', '112.208.64.132', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 5),
(419, '2025-05-09 16:28:44', '112.208.64.132', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 5),
(420, '2025-05-09 16:28:48', '112.208.64.132', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 5),
(421, '2025-05-09 16:33:37', '112.208.64.132', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 5),
(422, '2025-05-09 16:34:58', '112.208.64.132', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 5),
(423, '2025-05-09 16:37:19', '112.208.64.132', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 5),
(424, '2025-05-09 16:41:34', '112.208.64.132', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 5),
(425, '2025-05-09 16:43:38', '112.208.64.132', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 5),
(426, '2025-05-09 17:08:11', '112.208.64.132', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 5),
(427, '2025-05-09 17:10:11', '112.208.64.132', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(428, '2025-05-09 17:16:22', '112.208.64.132', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(429, '2025-05-09 17:31:58', '112.208.64.132', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(430, '2025-05-09 17:40:36', '112.208.64.132', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(431, '2025-05-09 17:41:34', '112.208.64.132', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 5),
(432, '2025-05-09 17:44:34', '112.208.64.132', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(433, '2025-05-09 17:53:54', '112.208.64.132', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 5),
(434, '2025-05-09 17:56:15', '112.208.64.132', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 5),
(435, '2025-05-09 17:57:36', '112.208.64.132', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(436, '2025-05-09 17:58:09', '112.208.64.132', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 5),
(437, '2025-05-09 18:03:58', '112.208.64.132', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 5),
(438, '2025-05-09 18:04:49', '112.208.64.132', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 5),
(439, '2025-05-09 18:11:35', '112.208.64.132', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 5),
(440, '2025-05-09 18:15:51', '112.208.64.132', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 5),
(441, '2025-05-09 18:17:46', '112.208.64.132', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 5),
(442, '2025-05-09 18:21:43', '112.208.64.132', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 5),
(443, '2025-05-09 18:22:43', '112.208.64.132', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(444, '2025-05-09 18:23:20', '112.208.64.132', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 5),
(445, '2025-05-09 18:29:00', '112.208.64.132', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 5),
(446, '2025-05-09 18:30:49', '112.208.64.132', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 5),
(447, '2025-05-10 18:04:27', '112.208.72.230', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(448, '2025-05-10 18:07:33', '112.208.72.230', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(449, '2025-05-10 18:09:36', '112.208.72.230', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(450, '2025-05-10 18:15:19', '112.208.72.230', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(451, '2025-05-10 18:22:10', '112.208.72.230', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(452, '2025-05-10 18:24:49', '112.208.72.230', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(453, '2025-05-10 18:28:10', '112.208.72.230', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(454, '2025-05-10 18:32:50', '112.208.72.230', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(455, '2025-05-10 18:38:22', '112.208.72.230', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(456, '2025-05-10 18:42:42', '112.208.72.230', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(457, '2025-05-10 18:44:49', '112.208.72.230', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(458, '2025-05-10 18:46:07', '112.208.72.230', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(459, '2025-05-10 18:58:20', '112.208.72.230', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(460, '2025-05-10 19:03:29', '112.208.72.230', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 5),
(461, '2025-05-10 19:16:50', '112.208.72.230', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 5),
(462, '2025-05-10 19:18:14', '112.208.72.230', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 5),
(463, '2025-05-11 04:57:13', '112.208.72.230', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(464, '2025-05-11 05:04:06', '112.208.72.230', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(465, '2025-05-11 05:24:21', '112.208.72.230', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(466, '2025-05-11 05:35:49', '112.208.72.230', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(467, '2025-05-11 05:46:52', '112.208.72.230', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(468, '2025-05-11 05:52:13', '112.208.72.230', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(469, '2025-05-11 05:58:17', '112.208.72.230', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(470, '2025-05-11 06:26:49', '112.208.72.230', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(471, '2025-05-11 06:30:04', '112.208.72.230', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(472, '2025-05-11 06:32:07', '112.208.72.230', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(473, '2025-05-11 06:36:39', '112.208.72.230', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(474, '2025-05-11 06:43:27', '112.208.72.230', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(475, '2025-05-11 06:57:01', '112.208.72.230', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(476, '2025-05-11 06:59:44', '112.208.72.230', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(477, '2025-05-11 07:10:17', '112.208.72.230', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(478, '2025-05-11 12:48:39', '112.208.72.230', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(479, '2025-05-11 16:07:04', '112.208.72.230', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(480, '2025-05-11 16:20:28', '112.208.72.230', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(481, '2025-05-11 16:22:39', '112.208.72.230', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(482, '2025-05-11 16:24:35', '112.208.72.230', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(483, '2025-05-11 16:29:50', '112.208.72.230', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(484, '2025-05-11 16:29:50', '112.208.72.230', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(485, '2025-05-11 16:40:39', '112.208.72.230', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(486, '2025-05-11 16:54:45', '112.208.72.230', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(487, '2025-05-11 16:58:32', '112.208.72.230', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(488, '2025-05-11 17:07:30', '112.208.72.230', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(489, '2025-05-11 17:13:11', '112.208.72.230', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(490, '2025-05-11 18:02:16', '112.208.72.230', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(491, '2025-05-11 18:08:14', '112.208.72.230', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(492, '2025-05-11 18:11:27', '112.208.72.230', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(493, '2025-05-11 18:13:11', '112.208.72.230', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(494, '2025-05-11 18:16:11', '112.208.72.230', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(495, '2025-05-11 18:17:18', '112.208.72.230', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(496, '2025-05-11 18:19:34', '112.208.72.230', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(497, '2025-05-11 18:26:29', '112.208.72.230', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(498, '2025-05-11 18:29:59', '112.208.72.230', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(499, '2025-05-11 18:31:49', '112.208.72.230', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(500, '2025-05-11 18:39:14', '112.208.72.230', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(501, '2025-05-11 18:44:20', '112.208.72.230', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(502, '2025-05-11 19:22:10', '112.208.72.230', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(503, '2025-05-11 19:26:57', '112.208.72.230', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(504, '2025-05-11 19:29:15', '112.208.72.230', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(505, '2025-05-11 19:32:14', '112.208.72.230', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(506, '2025-05-11 19:34:34', '112.208.72.230', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(507, '2025-05-11 19:37:13', '112.208.72.230', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(508, '2025-05-11 19:40:38', '112.208.72.230', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(509, '2025-05-11 19:43:24', '112.208.72.230', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(510, '2025-05-11 19:45:18', '112.208.72.230', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(511, '2025-05-11 19:52:25', '112.208.72.230', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(512, '2025-05-11 19:59:19', '112.208.72.230', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(513, '2025-05-11 20:01:03', '112.208.72.230', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(514, '2025-05-11 20:03:26', '112.208.72.230', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(515, '2025-05-11 20:08:35', '112.208.72.230', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(516, '2025-05-11 20:11:49', '112.208.72.230', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 5),
(517, '2025-05-11 20:15:06', '112.208.72.230', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 5),
(518, '2025-05-11 20:17:57', '112.208.72.230', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 5),
(519, '2025-05-11 20:18:00', '112.208.72.230', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 5),
(520, '2025-05-11 20:18:57', '112.208.72.230', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 5),
(521, '2025-05-11 20:22:12', '112.208.72.230', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 5),
(522, '2025-05-11 20:23:08', '112.208.72.230', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(523, '2025-05-11 20:25:56', '112.208.72.230', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 5),
(524, '2025-05-11 20:26:38', '112.208.72.230', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 5),
(525, '2025-05-11 20:27:05', '112.208.72.230', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(526, '2025-05-12 05:40:32', '112.208.72.230', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(527, '2025-05-12 05:42:29', '112.208.72.230', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(528, '2025-05-12 05:48:56', '112.208.72.230', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(529, '2025-05-12 05:50:45', '112.208.72.230', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(530, '2025-05-12 05:51:43', '112.208.72.230', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(531, '2025-05-12 05:58:39', '112.208.72.230', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(532, '2025-05-12 06:01:13', '112.208.72.230', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(533, '2025-05-12 06:04:28', '112.208.72.230', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(534, '2025-05-12 06:09:39', '112.208.72.230', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(535, '2025-05-12 06:10:55', '112.208.72.230', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(536, '2025-05-12 06:12:25', '112.208.72.230', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(537, '2025-05-12 06:14:55', '112.208.72.230', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(538, '2025-05-12 06:14:57', '112.208.72.230', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(539, '2025-05-12 06:16:25', '112.208.72.230', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(540, '2025-05-12 06:18:24', '112.208.72.230', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(541, '2025-05-12 06:19:24', '112.208.72.230', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(542, '2025-05-12 06:22:20', '112.208.72.230', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(543, '2025-05-12 06:25:33', '112.208.72.230', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(544, '2025-05-12 06:33:34', '112.208.72.230', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(545, '2025-05-12 06:41:41', '112.208.72.230', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(546, '2025-05-12 06:45:32', '112.208.72.230', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(547, '2025-05-12 06:48:26', '112.208.72.230', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(548, '2025-05-12 06:49:48', '112.208.72.230', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(549, '2025-05-12 06:53:53', '112.208.72.230', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(550, '2025-05-12 07:09:02', '112.208.72.230', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(551, '2025-05-12 07:11:16', '112.208.72.230', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(552, '2025-05-12 07:17:54', '112.208.72.230', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(553, '2025-05-12 07:35:59', '112.208.72.230', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(554, '2025-05-12 07:39:17', '112.208.72.230', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(555, '2025-05-12 07:50:44', '112.208.72.230', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(556, '2025-05-12 07:59:34', '112.208.72.230', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(557, '2025-05-12 08:18:27', '112.208.72.230', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(558, '2025-05-12 08:33:53', '112.208.72.230', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(559, '2025-05-12 08:46:54', '112.208.72.230', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(560, '2025-05-12 08:52:46', '112.208.72.230', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(561, '2025-05-12 08:58:15', '112.208.72.230', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(562, '2025-05-12 09:02:51', '112.208.72.230', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(563, '2025-05-12 09:52:27', '112.208.72.230', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(564, '2025-05-12 09:53:39', '112.208.72.230', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 5),
(565, '2025-05-12 10:44:14', '112.208.72.230', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(566, '2025-05-12 10:46:49', '112.208.72.230', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(567, '2025-05-12 10:53:19', '112.208.72.230', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(568, '2025-05-12 11:00:58', '112.208.72.230', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(569, '2025-05-12 11:03:34', '112.208.72.230', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(570, '2025-05-12 11:03:58', '112.208.72.230', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(571, '2025-05-12 11:05:41', '112.208.72.230', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(572, '2025-05-12 11:06:29', '112.208.72.230', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(573, '2025-05-12 11:07:10', '112.208.72.230', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(574, '2025-05-12 11:10:20', '112.208.72.230', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(575, '2025-05-12 11:49:24', '112.208.72.230', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(576, '2025-05-12 11:56:57', '112.208.72.230', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(577, '2025-05-12 12:04:13', '112.208.72.230', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(578, '2025-05-12 12:11:24', '112.208.72.230', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(579, '2025-05-12 12:17:44', '112.208.72.230', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(580, '2025-05-12 12:46:15', '112.208.72.230', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(581, '2025-05-12 12:52:32', '112.208.72.230', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(582, '2025-05-12 12:54:10', '112.208.72.230', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(583, '2025-05-12 13:22:46', '112.208.72.230', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(584, '2025-05-12 13:28:08', '112.208.72.230', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(585, '2025-05-12 13:30:16', '112.208.72.230', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(586, '2025-05-12 13:37:47', '112.208.72.230', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(587, '2025-05-12 13:42:31', '112.208.72.230', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(588, '2025-05-12 14:15:31', '112.208.72.230', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(589, '2025-05-12 14:20:27', '112.208.72.230', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(590, '2025-05-12 14:24:26', '112.208.72.230', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(591, '2025-05-12 14:36:39', '112.208.72.230', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(592, '2025-05-12 15:02:59', '112.208.72.230', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(593, '2025-05-12 15:09:44', '112.208.72.230', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(594, '2025-05-12 15:10:45', '112.208.72.230', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(595, '2025-05-12 15:18:09', '112.208.72.230', 'M2102J20SG / Android', 'Cebu City, Central Visayas, PH', 14),
(596, '2025-05-12 15:21:18', '112.208.72.230', 'M2102J20SG / Android', 'Cebu City, Central Visayas, PH', 14),
(597, '2025-05-12 15:26:43', '112.208.72.230', 'M2102J20SG / Android', 'Cebu City, Central Visayas, PH', 14),
(598, '2025-05-12 15:27:17', '112.208.72.230', 'M2102J20SG / Android', 'Cebu City, Central Visayas, PH', 14),
(599, '2025-05-12 15:31:53', '112.208.72.230', 'M2102J20SG / Android', 'Cebu City, Central Visayas, PH', 14),
(600, '2025-05-12 15:37:04', '112.208.72.230', 'M2102J20SG / Android', 'Cebu City, Central Visayas, PH', 14),
(601, '2025-05-12 15:38:29', '112.208.72.230', 'M2102J20SG / Android', 'Cebu City, Central Visayas, PH', 14),
(602, '2025-05-12 15:45:59', '112.208.72.230', 'M2102J20SG / Android', 'Cebu City, Central Visayas, PH', 14),
(603, '2025-05-12 15:46:22', '112.208.72.230', 'M2102J20SG / Android', 'Cebu City, Central Visayas, PH', 14),
(604, '2025-05-12 16:04:00', '112.208.72.230', 'M2102J20SG / Android', 'Cebu City, Central Visayas, PH', 14),
(605, '2025-05-12 16:08:31', '112.208.72.230', 'M2102J20SG / Android', 'Cebu City, Central Visayas, PH', 14),
(606, '2025-05-12 16:10:44', '112.208.72.230', 'M2102J20SG / Android', 'Cebu City, Central Visayas, PH', 14),
(607, '2025-05-12 16:12:07', '112.208.72.230', 'M2102J20SG / Android', 'Cebu City, Central Visayas, PH', 14),
(608, '2025-05-12 16:15:05', '112.208.72.230', 'M2102J20SG / Android', 'Cebu City, Central Visayas, PH', 14),
(609, '2025-05-12 16:20:15', '112.208.72.230', 'M2102J20SG / Android', 'Cebu City, Central Visayas, PH', 14),
(610, '2025-05-12 16:25:18', '112.208.72.230', 'M2102J20SG / Android', 'Cebu City, Central Visayas, PH', 14),
(611, '2025-05-12 16:29:49', '112.208.72.230', 'M2102J20SG / Android', 'Cebu City, Central Visayas, PH', 14),
(612, '2025-05-12 16:35:20', '112.208.72.230', 'M2102J20SG / Android', 'Cebu City, Central Visayas, PH', 14),
(613, '2025-05-12 16:36:45', '112.208.72.230', 'M2102J20SG / Android', 'Cebu City, Central Visayas, PH', 14),
(614, '2025-05-12 16:52:41', '112.208.72.230', 'M2102J20SG / Android', 'Cebu City, Central Visayas, PH', 14),
(615, '2025-05-12 18:27:09', '112.208.72.230', 'M2102J20SG / Android', 'Cebu City, Central Visayas, PH', 14),
(616, '2025-05-12 19:17:49', '112.208.72.230', 'M2102J20SG / Android', 'Cebu City, Central Visayas, PH', 14),
(617, '2025-05-12 19:23:23', '112.208.72.230', 'M2102J20SG / Android', 'Cebu City, Central Visayas, PH', 14),
(618, '2025-05-13 04:14:06', '112.208.72.230', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(619, '2025-05-13 04:21:16', '112.208.72.230', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(620, '2025-05-13 04:27:35', '112.208.72.230', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(621, '2025-05-13 04:30:17', '112.208.72.230', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(622, '2025-05-13 04:30:34', '112.208.72.230', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 5),
(623, '2025-05-13 04:34:28', '112.208.72.230', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 5),
(624, '2025-05-13 14:44:15', '112.208.72.230', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(625, '2025-05-15 07:52:15', '112.208.72.230', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(626, '2025-05-15 08:09:41', '112.208.72.230', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14),
(627, '2025-05-15 08:17:42', '112.208.72.230', 'sdk_gphone64_x86_64 / Android', 'Cebu City, Central Visayas, PH', 14);

-- --------------------------------------------------------

--
-- Table structure for table `user`
--

CREATE TABLE `user` (
  `user_id` int(11) NOT NULL,
  `firstname` varchar(50) NOT NULL,
  `lastname` varchar(50) NOT NULL,
  `username` varchar(50) NOT NULL,
  `password` varchar(50) NOT NULL,
  `role` varchar(50) NOT NULL,
  `email` varchar(50) NOT NULL,
  `level` int(11) DEFAULT 1,
  `created_at` timestamp NOT NULL DEFAULT current_timestamp(),
  `updated_at` timestamp NULL DEFAULT NULL ON UPDATE current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `user`
--

INSERT INTO `user` (`user_id`, `firstname`, `lastname`, `username`, `password`, `role`, `email`, `level`, `created_at`, `updated_at`) VALUES
(5, 'admin', 'admin', 'admin', 'admin123', 'Admin', 'crisdegumaomambac@gmail.com', 1, '2025-05-09 15:35:24', NULL),
(6, 'Shane', 'Tapasao', 'shane', 'shane123', 'Student', 'shane@gmail.com', 4, '2025-05-09 15:35:24', '2025-05-09 15:38:20'),
(7, 'Christianne', 'Sabio', 'chan', 'chan123', 'Student', 'chan@gmail.com', 1, '2025-05-09 15:35:24', NULL),
(10, 'Christianne', 'Sabio', 'chan321', 'chan321', 'Student', 'chan123@gmail.com', 3, '2025-05-09 15:35:24', NULL),
(14, 'Cris', 'Omambac', 'cris06', 'cris123', 'Student', 'crisdegumaomambac@gmail.com', 2, '2025-05-09 15:35:24', '2025-05-15 08:18:53'),
(16, 'qqq', 'qqqq', 'crisomambac28', 'cris123', 'Student', 'eswj@gmail.com', 1, '2025-05-11 16:13:41', NULL),
(17, 'sss', 'sss', 'sds213', 'wweeqe', 'Student', 'weqe@gmail.com', 1, '2025-05-11 16:16:27', NULL),
(18, 'ewqe', 'eqewq', 'eqwewqew', 'weqeieqw', 'Student', 'wqeqwe@gmail.com', 1, '2025-05-11 16:19:32', NULL);

--
-- Indexes for dumped tables
--

--
-- Indexes for table `leaderboard`
--
ALTER TABLE `leaderboard`
  ADD PRIMARY KEY (`id`),
  ADD KEY `user_id` (`user_id`);

--
-- Indexes for table `login_history`
--
ALTER TABLE `login_history`
  ADD PRIMARY KEY (`login_history_id`),
  ADD KEY `user_id` (`user_id`);

--
-- Indexes for table `user`
--
ALTER TABLE `user`
  ADD PRIMARY KEY (`user_id`);

--
-- AUTO_INCREMENT for dumped tables
--

--
-- AUTO_INCREMENT for table `leaderboard`
--
ALTER TABLE `leaderboard`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT for table `login_history`
--
ALTER TABLE `login_history`
  MODIFY `login_history_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=628;

--
-- AUTO_INCREMENT for table `user`
--
ALTER TABLE `user`
  MODIFY `user_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=19;

--
-- Constraints for dumped tables
--

--
-- Constraints for table `leaderboard`
--
ALTER TABLE `leaderboard`
  ADD CONSTRAINT `user_id` FOREIGN KEY (`user_id`) REFERENCES `user` (`user_id`);

--
-- Constraints for table `login_history`
--
ALTER TABLE `login_history`
  ADD CONSTRAINT `login_history_ibfk_1` FOREIGN KEY (`user_id`) REFERENCES `user` (`user_id`) ON DELETE CASCADE;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
