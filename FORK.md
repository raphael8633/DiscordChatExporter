# About this fork

This is a personal fork of [Tyrrrz/DiscordChatExporter](https://github.com/Tyrrrz/DiscordChatExporter), which is in [maintenance mode](https://github.com/Tyrrrz/.github/blob/prime/docs/project-status.md). It adds a few CLI options that make long-running JSON exports send far fewer requests. Nothing here is endorsed by the upstream author; please report problems with these additions to this fork, not upstream.

No binaries are published. Build from source (`dotnet publish DiscordChatExporter.Cli`) if you want to use it.

## Branches

| Branch | Content |
|---|---|
| `prime` | Mirror of upstream `prime`, kept in sync only |
| `raph/stable` | Latest upstream release tag (currently `2.48`) plus the modules below, each merged through its own pull request |
| `feature/*` | One branch per module, stacked in merge order |

When upstream publishes a new release tag, `raph/stable` is rebuilt on top of it. A module is dropped once upstream ships an equivalent.

## Modules

All options are CLI-only (the GUI is unchanged) and default to upstream behavior. Usage details are in [Using the CLI](.docs/Using-the-CLI.md).

| Option | Environment variable | What it does |
|---|---|---|
| `--skip-reaction-details` | – | JSON export keeps reaction emojis and counts but leaves `users` empty, skipping one request per emoji per message. Same name and behavior as upstream PR [#1599](https://github.com/Tyrrrz/DiscordChatExporter/pull/1599); dropped here if that is merged. |
| `--request-delay-min`, `--request-delay-max` | `DCE_JITTER_MS_MIN`, `DCE_JITTER_MS_MAX` | Random delay (ms) before every request, including retries. |
| `--member-snapshot` | `DCE_MEMBER_SNAPSHOT` | Reads guild members (nickname, roles) from a JSONL file captured earlier instead of requesting each author individually (see [#749](https://github.com/Tyrrrz/DiscordChatExporter/issues/749)). Entries older than 30 days are ignored; authors resolved this way get a `memberSnapshotAt` field in JSON output. |

## Tests

New unit tests (`RequestDelaySpecs`, `MemberSnapshotSpecs`, `CliOptionSpecs` in `DiscordChatExporter.Cli.Tests`) do not need a Discord token:

```console
dotnet test DiscordChatExporter.Cli.Tests --filter "FullyQualifiedName~RequestDelaySpecs|FullyQualifiedName~MemberSnapshotSpecs|FullyQualifiedName~CliOptionSpecs"
```

## License

MIT, same as upstream. See [License.txt](License.txt); the original copyright notice is kept.
