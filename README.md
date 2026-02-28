# Webhook Receiver & Allure Report
## Webhook Receiver (Render)

Webhook tests require a public HTTPS endpoint.
The webhook receiver is deployed on Render and is used to:

* Accept webhook POST requests from Scriptube

* Store incoming events

* Expose admin endpoints so tests can verify delivery

### Receiver URL

Configured via GitHub secret:
```
SCRIPTUBE_WEBHOOK_RECEIVER_URL
```
Example:
```
https://sted-prep.onrender.com
```
Webhook endpoint used by Scriptube:
```
POST /hook
```
Full URL:
```
https://sted-prep.onrender.com/hook
```
### Admin Access

Tests use admin endpoints to:

* Fetch last received event

* Clear stored events

Configured via:
```
SCRIPTUBE_WEBHOOK_RECEIVER_ADMIN_TOKEN
```
This token must also be set in Render environment variables.

## Allure Report (GitHub Pages)

Test results are generated using Allure.

Workflow steps:

1. Tests produce allure-results

2. GitHub Actions generates allure-report

3.  Report is deployed to GitHub Pages

### Accessing the Report

After a workflow run, open:
```
https://<your-github-username>.github.io/<repository-name>/
```
Example:
```
https://crisp-0136.github.io/homework/
```
If GitHub Pages is enabled, the latest test report will be available at that URL.