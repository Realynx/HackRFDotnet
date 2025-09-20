---
title: Welcome To HackRfDotnet
description: Getting started information for HackRfDotnet Api.
---

# What HackRfDotnet Does
HackRfDotnet is a managed .NET wrapper and library that exposes and extends the C library shipped with the HackRF from [Great Scott Gadgets](https://github.com/greatscottgadgets/hackrf).  
It allows developers to interact with HackRF devices using idiomatic .NET patterns, making it easier to prototype radio workflows, stream signals, and integrate HackRF functionality into custom applications.  

With HackRfDotnet, you can quickly set up device streams, apply signal processing pipelines, or integrate your HackRF device into dependency injection (DI) architectures for structured applications.  

With HackRfDotnet’s full-featured Digital Signal Processing (DSP) pipelines, you can configure and process a wide variety of signals, from analogue modulations such as FM and AM, to digital data streams including QAM, OFDM, BPSK, and QPSK.
These pipelines allow for real-time demodulation, filtering, and transformation, enabling both experimentation and production-grade radio workflows.

# Example Code Documentation
Below is a table of contents linking to our documentation and example programs, showing both basic usage and advanced scenarios.

## Getting Started
00 [Setting Up Radio](./01-start.html) – Learn how to initialize and configure your HackRF device.  
02 [Setting Up Radio In DI](./02-di.html) – Using HackRfDotnet with dependency injection for managed device lifecycles and service integration.  

## Advanced
05 [Scanning Frequencies](./getting-started.html)  


> [!WARNING]
> **Legal Notice:** HackRF devices are capable of transmitting and receiving radio signals. Use of these devices is subject to local, regional, and national laws and regulations. HackRfDotnet and its maintainers ([Realynx](https://github.com/Realynx/)) provide this library for **educational, experimental, and research purposes only**.  
>
> **You are solely responsible** for any use of your HackRF device, including compliance with spectrum licensing, transmission power limits, and prohibited frequency bands. HackRfDotnet, Realynx (including its members), and [Great Scott Gadgets](https://github.com/greatscottgadgets/hackrf) **are not responsible** for damages, legal penalties, or regulatory violations resulting from improper or unlawful use.  
>
> **Always verify your local laws** before transmitting, and do not transmit on frequencies for which you do not have authorization.
