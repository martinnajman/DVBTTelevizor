﻿using System;
using System.Collections.Generic;
using System.Text;

namespace MPEGTS
{
    public class NITTable : DVBTTable
    {
        public string NetworkName { get; set; }

        public int TransportStreamId { get; set; }
        public int OriginalNetworkId { get; set; }
        public int ServiceId { get; set; }
        public byte LinkageType { get; set; }

        public static NITTable Parse(List<byte> bytes)
        {
            if (bytes == null || bytes.Count < 5)
                return null;

            var res = new NITTable();

            var pointerFiled = bytes[0];
            var pos = 1 + pointerFiled;

            if (bytes.Count < pos + 2)
                return null;

            res.ID = bytes[pos];

            // read next 2 bytes
            var tableHeader1 = bytes[pos + 1];
            var tableHeader2 = bytes[pos + 2];

            res.SectionSyntaxIndicator = ((tableHeader1 & 128) == 128);
            res.Private = ((tableHeader1 & 64) == 64);
            res.Reserved = Convert.ToByte((tableHeader1 & 48) >> 4);
            res.SectionLength = Convert.ToInt32(((tableHeader1 & 15) << 8) + tableHeader2);

            pos = pos + 3;

            res.NetworkID = (bytes[pos + 0] << 8) + bytes[pos + 1];

            pos = pos + 2;

            res.Version = Convert.ToByte((bytes[pos + 0] & 64) >> 1);
            res.CurrentIndicator = (bytes[pos + 0] & 1) == 1;

            res.SectionNumber = bytes[pos + 1];
            res.LastSectionNumber = bytes[pos + 2];

            pos = pos + 3;

            var networkDecriptorsLength = Convert.ToInt32(((bytes[pos + 0] & 15) << 8) + bytes[pos + 1]);

            pos = pos + 2;

            // network decriptors folowing

            var posAfterNetworkDescriptors = pos + networkDecriptorsLength;

            while (pos < posAfterNetworkDescriptors)
            {
                var descriptorTag = bytes[pos + 0];
                var descriptorLength = bytes[pos + 1];

                if (descriptorTag == 64)
                {
                    // 64 (dec) 40 (hex) - network_name_descriptor
                    res.NetworkName = GetStringFromByteArray(bytes.ToArray(), pos + 2, descriptorLength);
                }

                if (descriptorTag == 74)
                {
                    // 74 (dec) 4A (hex) - linkage_descriptor
                    res.TransportStreamId = Convert.ToInt32(((bytes[pos + 2]) << 8) + bytes[pos + 3]);
                    res.OriginalNetworkId = Convert.ToInt32(((bytes[pos + 4]) << 8) + bytes[pos + 5]);
                    res.ServiceId = Convert.ToInt32(((bytes[pos + 6]) << 8) + bytes[pos + 7]);
                    res.LinkageType = bytes[pos + 8];
                }

                pos += descriptorLength + 2;
            }

            var transportStreamLoopLength = Convert.ToInt32(((bytes[pos + 0] & 15) << 8) + bytes[pos + 1]);

            pos += 2;

            var posAfterTransportStreams = pos + transportStreamLoopLength;

            while (pos < posAfterTransportStreams)
            {
                var transportStreamId = Convert.ToInt32(((bytes[pos + 0]) << 8) + bytes[pos + 1]);
                var originalNetworkId = Convert.ToInt32(((bytes[pos + 2]) << 8) + bytes[pos + 3]);
                var transportDescriptorsLength = Convert.ToInt32(((bytes[pos + 4] & 15) << 8) + bytes[pos + 5]);

                pos += 6;

                var posAfterTransportDescriptors = pos + transportDescriptorsLength;

                while (pos < posAfterTransportDescriptors)
                {
                    var descriptorTag = bytes[pos + 0];
                    var descriptorLength = bytes[pos + 1];

                    Console.WriteLine($"Found descriptor: {descriptorTag}");
                    Console.WriteLine($"          length: {descriptorLength}");
                    Console.WriteLine($"        position: {pos + 2}");
                    Console.WriteLine($"------------------------------------");

                    // TODO: read descriptors of given descriptorTag
                    // 90 (dec) 5A (hex)  - terrestrial_delivery_system_descriptor
                    // 98 (dec) 62 (hex) - frequency_list_descriptor

                    pos += descriptorLength + 2;
                }
            }

            return res;
         }

        public void WriteToConsole()
        {
            Console.WriteLine($"ID                    : {ID}");
            Console.WriteLine($"SectionSyntaxIndicator: {SectionSyntaxIndicator}");
            Console.WriteLine($"Private               : {Private}");
            Console.WriteLine($"Reserved              : {Reserved}");
            Console.WriteLine($"SectionLength         : {SectionLength}");

            if (SectionSyntaxIndicator)
            {
                Console.WriteLine($"Version                : {Version}");
                Console.WriteLine($"CurrentIndicator       : {CurrentIndicator}");
                Console.WriteLine($"SectionNumber          : {SectionNumber}");
                Console.WriteLine($"LastSectionNumber      : {LastSectionNumber}");
            }

            Console.WriteLine($"NetworkName            : {NetworkName}");

            Console.WriteLine($"TransportStreamId      : {TransportStreamId}");
            Console.WriteLine($"OriginalNetworkId      : {OriginalNetworkId}");
            Console.WriteLine($"ServiceId              : {ServiceId}");
            Console.WriteLine($"LinkageType            : {LinkageType}");

        }
    }
}