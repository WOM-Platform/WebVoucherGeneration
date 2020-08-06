SHELL := /bin/bash

DC := docker-compose -f docker-compose.yml
DC_RUN := ${DC} run --rm

include config.env
export

.PHONY: confirmation
confirmation:
	@echo -n 'Are you sure? [y|N] ' && read ans && [ $$ans == y ]

.PHONY: cmd
cmd:
	@echo 'Docker-Compose command:'
	@echo '${DC}'

.PHONY: up
up:
	${DC} up -d site
	${DC} ps

.PHONY: ps
ps:
	${DC} ps

.PHONY: rs
rs:
	${DC} restart

.PHONY: rebuild
rebuild:
	${DC} rm -sf site
	${DC} build site
	${DC} up -d site

.PHONY: stop
stop:
	${DC} stop

.PHONY: rm
rm:
	${DC} rm -fs

.PHONY: logs
logs:
	docker logs -f $(shell ${DC} ps -q site)
