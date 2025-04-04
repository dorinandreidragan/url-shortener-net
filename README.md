# URL Shortener - **url-shortener-net**

![Build Status](https://github.com/dorinandreidragan/url-shortener-net/actions/workflows/ci.yml/badge.svg)

Welcome to the **URL Shortener** project! This repository is part of the **Nuts and Bolts of System Design** series. It showcases the iterative development of a scalable and reliable URL shortener system, built using **C# .NET Minimal Web API**.

## 🚀 About the Project

The **URL Shortener** is a practical example to explore **system design principles**. It starts as a simple single-server solution and evolves to tackle scalability, reliability, and performance challenges.

### Key Features

- **URL Shortening**: Convert long URLs into short, easily shareable links.
- **Redirection**: Seamless redirect from the shortened URL to the original one.
- **Hands-On Iterative Process**: The project evolves incrementally, solving challenges step-by-step.

## 🎯 Functional Requirements

1. **Shorten a URL**: Convert `https://example.com/long-url` into `https://short.ly/abc123`.
2. **Redirect**: Access `https://short.ly/abc123` redirects to the original URL.

## 💡 Non-Functional Requirements

1. **Low latency**: Fast redirection ⚡.
2. **Scalability**: Designed to handle millions of requests 📈.
3. **Reliability**: Prevent downtime and broken links 🔗.

## 🔧 Hands-On Iterative Process

This project is implemented in **rounds**, with each round focusing on new challenges and solutions. The code for each round is maintained in **separate branches** to track iterative progress.

The first round is already implemented and available on the branch [round-01-the-basics].

### Upcoming Rounds:

- Persistent Storage (SQL/NoSQL databases)
- Caching Mechanisms
- Distributed Architecture

---

## 📦 How to Run

### Prerequisites:

- .NET SDK 9.0+
- Docker (optional, for future containerized versions)

### Installation:

1. Clone the repository:

   ```bash
   git clone https://github.com/dorinandreidragan/url-shortener-net.git
   cd url-shortener-net
   ```

2. Run the application:

   ```bash
   dotnet run
   ```

3. Access endpoints:
   - **Shorten a URL**: POST `/shorten`
   - **Redirect**: GET `/{shortKey}`

---

## 📖 Series Outline

This project is part of the **Nuts and Bolts of System Design** series, exploring practical system design examples. Each round introduces refinements to handle real-world challenges.

---

## 🤝 Contributing

We welcome contributions to improve functionality, scalability, or add new features! Feel free to fork the repo and create pull requests.

---

## 📄 License

This project is licensed under the [MIT License](LICENSE).

---

## 🌟 Connect

- Share your feedback, suggestions, or challenges via [GitHub Issues](https://github.com/dorinandreidragna/url-shortener-net/issues).
- Follow the series for updates on upcoming rounds!

[round-01-the-basics]: https://github.com/dorinandreidragan/url-shortener-net/tree/round-01-the-basics
