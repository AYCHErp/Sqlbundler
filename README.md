**How Sql Bundler Works?**

Create a `.sqlbundle` file (`yaml` format) on your db directory.

```yaml
- script-directory : db/1.x/1.0/src
- output-directory : db/1.x/1.0
```


* **script-directory**: The path containing your SQL files. Every single file having `.sql` (but not `.sample.sql`) extension is merged together to create a bundle.
* **output-directory**: The directory where the bundled file will be created. The bundled file will have the filename of `.sqlbundle` file.

**Syntax**

```
path-to\SqlBundler.exe root-path sqlbundle-directory include_sample
```

**Example**

```
path-to\SqlBundler.exe "C:\frapid" "db/meta/1.x/1.0" false
```